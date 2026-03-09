import { createBrowserRouter } from "react-router";
import MainChat from "./pages/MainChat";
import VoiceActive from "./pages/VoiceActive";
import Settings from "./pages/Settings";
import OnboardingScreen from "./pages/OnboardingScreen";
import AnalyticsScreen from "./pages/AnalyticsScreen";
import QuickActionCustomizer from "./pages/QuickActionCustomizer";

export const router = createBrowserRouter([
  {
    path: "/",
    Component: MainChat,
  },
  {
    path: "/voice",
    Component: VoiceActive,
  },
  {
    path: "/settings",
    Component: Settings,
  },
  {
    path: "/onboarding",
    Component: OnboardingScreen,
  },
  {
    path: "/analytics",
    Component: AnalyticsScreen,
  },
  {
    path: "/customize-actions",
    Component: QuickActionCustomizer,
  }
]);
