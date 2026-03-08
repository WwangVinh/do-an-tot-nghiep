import { useState, useEffect } from "react";
import { RouterProvider } from "react-router";
import { HomePage } from "./components/HomePage";
import { AuthPage } from "./components/AuthPage";
import { router } from "./routes";
import { Toaster } from "sonner";

export default function App() {
  const [showAuth, setShowAuth] = useState(false);
  const [authKey, setAuthKey] = useState(0);
  const [isAdminRoute, setIsAdminRoute] = useState(false);

  useEffect(() => {
    // Check if current path is admin route
    setIsAdminRoute(window.location.pathname.startsWith("/admin"));
  }, []);

  const handleSuccessfulLogin = () => {
    console.log("handleSuccessfulLogin called!");
    setShowAuth(false);
    // Trigger re-render by updating key
    setAuthKey((prev) => prev + 1);
  };

  const handleLogout = () => {
    // Force re-render after logout
    setAuthKey((prev) => prev + 1);
  };

  // If admin route, use React Router
  if (isAdminRoute) {
    return (
      <>
        <Toaster position="top-right" richColors />
        <RouterProvider router={router} />
      </>
    );
  }

  // Otherwise, use the existing public pages
  return (
    <>
      <Toaster position="top-right" richColors />
      <div className="size-full">
        {showAuth ? (
          <AuthPage onBackToHome={handleSuccessfulLogin} />
        ) : (
          <HomePage 
            key={authKey} 
            onLoginClick={() => setShowAuth(true)} 
            onLogout={handleLogout}
          />
        )}
      </div>
    </>
  );
}