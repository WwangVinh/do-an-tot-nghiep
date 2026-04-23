import { useCallback, useEffect, useRef, useState } from 'react'
import { Bot, ChevronRight, MessageCircle, Send, X } from 'lucide-react'
import { Link } from 'react-router-dom'

import { env } from '../lib/env'
import { http } from '../services/http/http'

type ChatRole = 'user' | 'assistant'

type AiCarCard = {
  carId: number
  name: string
  price: number | null
  imageUrl: string | null
}

type Turn = { role: ChatRole; content: string; cards?: AiCarCard[] }

type ChatResponse = { reply: string; suggestedCarIds?: number[] }

type CarDetailPayload = {
  carId: number
  name: string
  price: number | null
  imageUrl: string | null
}

function toAbsoluteMedia(path: string | null | undefined) {
  const raw = (path ?? '').trim()
  if (!raw) return ''
  return new URL(raw, env.VITE_API_BASE_URL).toString()
}

function formatVnd(price: number | null | undefined) {
  if (!Number.isFinite(price ?? NaN)) return 'Liên hệ'
  return new Intl.NumberFormat('vi-VN').format(price as number) + ' ₫'
}

async function fetchCarCardsForAi(ids: number[]): Promise<AiCarCard[]> {
  const unique = [...new Set(ids.filter((x) => Number.isInteger(x) && x > 0))].slice(0, 8)
  if (unique.length === 0) return []

  const settled = await Promise.allSettled(
    unique.map((id) =>
      http.get<{ data: CarDetailPayload }>(`/api/Cars/${id}`).then((r) => r.data.data),
    ),
  )

  const out: AiCarCard[] = []
  for (const s of settled) {
    if (s.status !== 'fulfilled') continue
    const d = s.value
    if (!d?.carId) continue
    out.push({
      carId: d.carId,
      name: d.name,
      price: d.price ?? null,
      imageUrl: d.imageUrl ?? null,
    })
  }
  return out
}

export function AiChatPanel() {
  const [open, setOpen] = useState(false)
  const [input, setInput] = useState('')
  const [loading, setLoading] = useState(false)
  const [messages, setMessages] = useState<Turn[]>([
    {
      role: 'assistant',
      content:
        'Xin chào! Mình là trợ lý AI. Bạn cần tư vấn về xe, giá hay tồn kho? (Thông tin dựa trên dữ liệu trên hệ thống.)',
    },
  ])
  const listRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    if (!open) return
    listRef.current?.scrollTo({ top: listRef.current.scrollHeight, behavior: 'smooth' })
  }, [messages, open, loading])

  const send = useCallback(async () => {
    const text = input.trim()
    if (!text || loading) return

    setInput('')
    const historyForApi = messages.map((m) => ({ role: m.role, content: m.content }))
    setMessages((prev) => [...prev, { role: 'user', content: text }])
    setLoading(true)

    try {
      const { data } = await http.post<ChatResponse>(
        '/api/AiAdvisor/chat',
        {
          message: text,
          history: historyForApi,
        },
        { timeout: 90_000 },
      )
      const reply = data.reply?.trim() || 'Không có nội dung phản hồi.'
      const ids = data.suggestedCarIds ?? []
      const cards = ids.length > 0 ? await fetchCarCardsForAi(ids) : []
      setMessages((prev) => [...prev, { role: 'assistant', content: reply, cards }])
    } catch (e: unknown) {
      const errData =
        e && typeof e === 'object' && 'response' in e
          ? (e as { response?: { data?: { message?: string; detail?: string } } }).response?.data
          : undefined
      const serverMsg = errData?.message
      const serverDetail = errData?.detail
      const detailSnippet =
        typeof serverDetail === 'string' && serverDetail.length > 0
          ? serverDetail.length > 280
            ? `${serverDetail.slice(0, 280)}…`
            : serverDetail
          : ''
      const line =
        typeof serverMsg === 'string' && serverMsg.length > 0
          ? detailSnippet
            ? `${serverMsg}\n\n${detailSnippet}`
            : serverMsg
          : 'Không gửi được tin. Kiểm tra backend đang chạy và cấu hình AiAdvisor (Provider + GeminiApiKey hoặc OpenAIApiKey).'
      setMessages((prev) => [
        ...prev,
        {
          role: 'assistant',
          content: `${line} Nếu cần gấp, bạn có thể gọi hotline hoặc nhắn Zalo ở góc trái màn hình.`,
        },
      ])
    } finally {
      setLoading(false)
    }
  }, [input, loading, messages])

  return (
    <>
      <div
        className="pointer-events-none fixed bottom-4 right-4 z-[100] flex flex-col items-end gap-3 sm:bottom-6 sm:right-6"
        style={{
          paddingBottom: 'max(0px, env(safe-area-inset-bottom))',
          paddingRight: 'max(0px, env(safe-area-inset-right))',
        }}
      >
        {open ? (
          <div
            className="pointer-events-auto flex max-h-[min(560px,calc(100dvh-6rem))] w-[min(100vw-2rem,380px)] flex-col overflow-hidden rounded-2xl border border-zinc-200/80 bg-white shadow-[0_20px_50px_rgba(0,0,0,0.18)]"
            role="dialog"
            aria-label="Trò chuyện tư vấn AI"
          >
            <div className="flex items-center justify-between gap-2 border-b border-zinc-100 bg-gradient-to-r from-zinc-900 to-zinc-800 px-4 py-3 text-white">
              <div className="flex min-w-0 items-center gap-2">
                <span className="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-white/10">
                  <Bot className="h-5 w-5" aria-hidden />
                </span>
                <div className="min-w-0">
                  <p className="truncate text-sm font-semibold leading-tight">Tư vấn AI</p>
                  <p className="truncate text-xs text-white/70">Theo dữ liệu xe trên hệ thống</p>
                </div>
              </div>
              <button
                type="button"
                onClick={() => setOpen(false)}
                className="flex h-9 w-9 shrink-0 items-center justify-center rounded-full text-white/90 hover:bg-white/10"
                aria-label="Đóng khung chat"
              >
                <X className="h-5 w-5" />
              </button>
            </div>

            <div ref={listRef} className="flex min-h-0 flex-1 flex-col gap-3 overflow-y-auto bg-zinc-50/80 px-3 py-3">
              {messages.map((m, i) => (
                <div
                  key={`${i}-${m.role}`}
                  className={`flex ${m.role === 'user' ? 'justify-end' : 'justify-start'}`}
                >
                  <div
                    className={`max-w-[92%] rounded-2xl px-3.5 py-2.5 text-sm leading-relaxed shadow-sm ${
                      m.role === 'user'
                        ? 'bg-[#e30613] text-white'
                        : 'border border-zinc-200/80 bg-white text-zinc-900'
                    }`}
                  >
                    <div className={m.role === 'assistant' ? 'whitespace-pre-wrap' : ''}>{m.content}</div>
                    {m.role === 'assistant' && m.cards && m.cards.length > 0 ? (
                      <div className="mt-2.5 space-y-2 border-t border-zinc-100 pt-2.5">
                        <p className="text-[11px] font-medium uppercase tracking-wide text-zinc-500">Xem nhanh</p>
                        <div className="flex max-h-48 flex-col gap-2 overflow-y-auto pr-0.5">
                          {m.cards.map((c) => (
                            <Link
                              key={c.carId}
                              to={`/san-pham/xe/${c.carId}`}
                              className="group flex gap-2 rounded-xl border border-zinc-200/90 bg-zinc-50/80 p-1.5 transition hover:border-zinc-300 hover:bg-white"
                              title="Mở trang chi tiết xe"
                            >
                              {c.imageUrl ? (
                                <img
                                  src={toAbsoluteMedia(c.imageUrl)}
                                  alt=""
                                  className="h-14 w-[4.5rem] shrink-0 rounded-lg object-cover"
                                />
                              ) : (
                                <div className="flex h-14 w-[4.5rem] shrink-0 items-center justify-center rounded-lg bg-zinc-200/80 text-[10px] text-zinc-500">
                                  Ảnh
                                </div>
                              )}
                              <div className="flex min-w-0 flex-1 flex-col py-0.5">
                                <p className="line-clamp-2 text-xs font-semibold text-zinc-900">{c.name}</p>
                                <p className="mt-0.5 text-[11px] text-zinc-600">{formatVnd(c.price)}</p>
                                <span className="mt-1.5 inline-flex items-center gap-0.5 text-[11px] font-semibold text-[#e30613] underline-offset-2 group-hover:underline">
                                  Chi tiết xe
                                  <ChevronRight className="h-3.5 w-3.5 shrink-0" aria-hidden />
                                </span>
                              </div>
                            </Link>
                          ))}
                        </div>
                      </div>
                    ) : null}
                  </div>
                </div>
              ))}
              {loading ? (
                <p className="text-xs text-zinc-500">Đang trả lời…</p>
              ) : null}
            </div>

            <div className="border-t border-zinc-100 bg-white p-3">
              <p className="mb-2 text-[11px] leading-snug text-zinc-500">
                Gợi ý mang tính tham khảo; giá và chính sách có thể đổi — vui lòng xác nhận với showroom.
              </p>
              <div className="flex gap-2">
                <input
                  type="text"
                  value={input}
                  onChange={(e) => setInput(e.target.value)}
                  onKeyDown={(e) => {
                    if (e.key === 'Enter' && !e.shiftKey) {
                      e.preventDefault()
                      void send()
                    }
                  }}
                  placeholder="Nhập câu hỏi…"
                  className="min-w-0 flex-1 rounded-xl border border-zinc-200 bg-zinc-50 px-3 py-2.5 text-sm text-black outline-none ring-0 placeholder:text-zinc-400 focus:border-zinc-400 focus:bg-white"
                  disabled={loading}
                  autoComplete="off"
                />
                <button
                  type="button"
                  onClick={() => void send()}
                  disabled={loading || !input.trim()}
                  className="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-zinc-900 text-white shadow-sm transition hover:bg-zinc-800 disabled:cursor-not-allowed disabled:opacity-40"
                  aria-label="Gửi"
                >
                  <Send className="h-4 w-4" />
                </button>
              </div>
            </div>
          </div>
        ) : null}

        <button
          type="button"
          onClick={() => setOpen((o) => !o)}
          className="pointer-events-auto flex h-14 w-14 items-center justify-center rounded-full bg-zinc-900 text-white shadow-[0_10px_30px_rgba(0,0,0,0.35)] ring-2 ring-white/30 transition hover:scale-105 hover:bg-zinc-800 active:scale-95"
          aria-expanded={open}
          aria-label={open ? 'Đóng tư vấn AI' : 'Mở tư vấn AI'}
        >
          {open ? <X className="h-6 w-6" /> : <MessageCircle className="h-6 w-6" />}
        </button>
      </div>
    </>
  )
}
