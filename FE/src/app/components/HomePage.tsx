import { Header } from "./Header";
import { HeroSlider } from "./HeroSlider";
import { CarGrid } from "./CarGrid";
import { Footer } from "./Footer";

interface HomePageProps {
  onLoginClick: () => void;
  onLogout?: () => void;
}

export function HomePage({ onLoginClick, onLogout }: HomePageProps) {
  return (
    <div className="min-h-screen bg-white">
      <Header onLoginClick={onLoginClick} onLogout={onLogout} />
      <main>
        <HeroSlider />
        <CarGrid />
      </main>
      <Footer />
    </div>
  );
}