import { InstallmentBenefitsSection } from './components/InstallmentBenefitsSection'
import { InstallmentDocumentsSection } from './components/InstallmentDocumentsSection'
import { InstallmentRegisterSection } from './components/InstallmentRegisterSection'

export function InstallmentPage() {
  return (
    <main className="w-full bg-white">
      <InstallmentRegisterSection />
      <InstallmentBenefitsSection />
      <InstallmentDocumentsSection />
    </main>
  )
}
