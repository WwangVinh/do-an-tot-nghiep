import { motion } from "motion/react";
import { Card } from "./ui/card";

const cars = [
  {
    id: 1,
    name: "CITY",
    price: "499.000.000",
    currency: "VND",
    image: "https://images.unsplash.com/photo-1658215704824-88a80b76c98b?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxob25kYSUyMGNpdHklMjByZWQlMjBjYXJ8ZW58MXx8fHwxNzcyNzM0MDUwfDA&ixlib=rb-4.1.0&q=80&w=1080",
    label: ""
  },
  {
    id: 2,
    name: "BR-V",
    price: "629.000.000",
    currency: "VND",
    image: "https://images.unsplash.com/photo-1603465833396-5ee350acca47?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxob25kYSUyMGJydiUyMHdoaXRlJTIwc3V2fGVufDF8fHx8MTc3MjczNDA1MHww&ixlib=rb-4.1.0&q=80&w=1080",
    label: ""
  },
  {
    id: 3,
    name: "HR-V",
    price: "699.000.000",
    currency: "VND",
    image: "https://images.unsplash.com/photo-1603465833396-5ee350acca47?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxob25kYSUyMGhydiUyMGJlaWdlJTIwc3V2fGVufDF8fHx8MTc3MjczNDA1MXww&ixlib=rb-4.1.0&q=80&w=1080",
    label: ""
  },
  {
    id: 4,
    name: "CIVIC",
    price: "789.000.000",
    currency: "VND",
    image: "https://images.unsplash.com/photo-1770750942761-a2dd38c7b68b?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxob25kYSUyMGNpdmljJTIwYmx1ZSUyMHNlZGFufGVufDF8fHx8MTc3MjczNDA1MXww&ixlib=rb-4.1.0&q=80&w=1080",
    label: ""
  },
  {
    id: 5,
    name: "CR-V",
    price: "1.029.000.000",
    currency: "VND",
    image: "https://images.unsplash.com/photo-1720236177685-b62ffa6377aa?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxob25kYSUyMGNydiUyMHJlZCUyMHN1dnxlbnwxfHx8fDE3NzI3MzQwNTJ8MA&ixlib=rb-4.1.0&q=80&w=1080",
    label: ""
  },
  {
    id: 6,
    name: "CIVIC TYPE R",
    price: "2.999.000.000",
    currency: "VND",
    image: "https://images.unsplash.com/photo-1686074449582-6374eaebacf3?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxob25kYSUyMGNpdmljJTIwdHlwZSUyMHJ8ZW58MXx8fHwxNzcyNzM0MDUzfDA&ixlib=rb-4.1.0&q=80&w=1080",
    label: "NEW"
  }
];

export function CarGrid() {
  return (
    <div className="bg-white py-16">
      <div className="mx-auto max-w-7xl px-4">
        <div className="grid grid-cols-1 gap-8 md:grid-cols-2 lg:grid-cols-3">
          {cars.map((car, index) => (
            <motion.div
              key={car.id}
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: index * 0.1 }}
            >
              <Card className="group cursor-pointer overflow-hidden border-0 shadow-lg transition-all hover:shadow-2xl">
                <div className="relative">
                  {car.label && (
                    <div className="absolute left-4 top-4 z-10 rounded bg-red-600 px-3 py-1 text-xs font-bold text-white">
                      {car.label}
                    </div>
                  )}
                  <div className="aspect-[4/3] overflow-hidden bg-gradient-to-br from-gray-50 to-gray-100">
                    <img
                      src={car.image}
                      alt={car.name}
                      className="h-full w-full object-cover transition-transform duration-500 group-hover:scale-110"
                    />
                  </div>
                </div>
                <div className="p-6">
                  <h3 className="mb-2 text-2xl font-bold text-red-600">{car.name}</h3>
                  <div className="flex items-baseline gap-2">
                    <span className="text-sm text-gray-500">Giá từ</span>
                    <span className="text-xl font-bold text-gray-900">{car.price}</span>
                    <span className="text-sm text-gray-500">{car.currency}</span>
                  </div>
                  <div className="mt-4 flex gap-2">
                    <button className="flex-1 rounded bg-red-600 px-4 py-2 text-sm font-semibold text-white transition hover:bg-red-700">
                      Xem chi tiết
                    </button>
                    <button className="flex-1 rounded border-2 border-red-600 px-4 py-2 text-sm font-semibold text-red-600 transition hover:bg-red-50">
                      Đăng ký lái thử
                    </button>
                  </div>
                </div>
              </Card>
            </motion.div>
          ))}
        </div>
      </div>
    </div>
  );
}
