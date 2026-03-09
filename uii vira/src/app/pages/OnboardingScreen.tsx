import { useState } from 'react';
import { motion, AnimatePresence } from 'motion/react';
import { useNavigate } from 'react-router';
import { Sparkles, Mic, Calendar, Globe, ArrowRight } from 'lucide-react';

const steps = [
  {
    title: "Meet Vira",
    subtitle: "Your new personal AI assistant.",
    description: "Vira is designed to make your daily life smoother, more organized, and surprisingly delightful.",
    icon: Sparkles,
    color: "from-purple-500 to-indigo-500",
  },
  {
    title: "Voice First",
    subtitle: "Talk to Vira naturally.",
    description: "Just tap the mic and speak. Vira understands context, follow-ups, and natural language perfectly.",
    icon: Mic,
    color: "from-blue-500 to-cyan-400",
  },
  {
    title: "Stay Organized",
    subtitle: "Schedules & Reminders.",
    description: "Never miss a beat. Vira syncs with your calendar and nudges you exactly when you need it.",
    icon: Calendar,
    color: "from-emerald-400 to-teal-500",
  },
  {
    title: "Always Connected",
    subtitle: "Real-time info.",
    description: "From live weather updates to the latest news, Vira pulls real-time data to keep you informed.",
    icon: Globe,
    color: "from-amber-400 to-orange-500",
  }
];

export default function OnboardingScreen() {
  const [currentStep, setCurrentStep] = useState(0);
  const navigate = useNavigate();

  const handleNext = () => {
    if (currentStep < steps.length - 1) {
      setCurrentStep(prev => prev + 1);
    } else {
      navigate('/');
    }
  };

  const handleSkip = () => {
    navigate('/');
  };

  const step = steps[currentStep];
  const Icon = step.icon;

  return (
    <div className="fixed inset-0 bg-[#101622] flex flex-col font-sans overflow-hidden">
      {/* Background Orbs */}
      <div className="absolute inset-0 pointer-events-none overflow-hidden">
        <AnimatePresence mode="popLayout">
          <motion.div
            key={currentStep}
            initial={{ opacity: 0, scale: 0.8 }}
            animate={{ opacity: 0.15, scale: 1 }}
            exit={{ opacity: 0, scale: 1.2 }}
            transition={{ duration: 1 }}
            className={`absolute top-[20%] left-1/2 -translate-x-1/2 w-[300px] h-[300px] rounded-full bg-gradient-to-tr ${step.color} blur-[80px]`}
          />
        </AnimatePresence>
      </div>

      {/* Header */}
      <div className="w-full pt-12 px-6 flex justify-between items-center relative z-10">
        <div className="flex gap-1.5">
          {steps.map((_, i) => (
            <div
              key={i}
              className={`h-1 rounded-full transition-all duration-300 ${
                i === currentStep ? 'w-6 bg-white' : 'w-2 bg-white/20'
              }`}
            />
          ))}
        </div>
        <button
          onClick={handleSkip}
          className="text-sm font-medium text-slate-400 hover:text-white transition-colors"
        >
          Skip
        </button>
      </div>

      {/* Content */}
      <div className="flex-1 flex flex-col items-center justify-center px-8 relative z-10">
        <AnimatePresence mode="wait">
          <motion.div
            key={currentStep}
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -20 }}
            transition={{ duration: 0.4 }}
            className="flex flex-col items-center text-center"
          >
            <div className={`w-24 h-24 rounded-3xl mb-8 flex items-center justify-center bg-gradient-to-tr ${step.color} p-[2px]`}>
              <div className="w-full h-full bg-[#101622] rounded-[22px] flex items-center justify-center relative overflow-hidden">
                <div className={`absolute inset-0 bg-gradient-to-tr ${step.color} opacity-20`} />
                <Icon className="w-10 h-10 text-white relative z-10" />
              </div>
            </div>

            <h2 className="text-3xl font-bold text-white mb-3 tracking-tight">
              {step.title}
            </h2>
            <h3 className="text-lg font-medium text-indigo-300 mb-4">
              {step.subtitle}
            </h3>
            <p className="text-[15px] text-slate-400 leading-relaxed max-w-[280px]">
              {step.description}
            </p>
          </motion.div>
        </AnimatePresence>
      </div>

      {/* Footer */}
      <div className="w-full px-6 pb-12 relative z-10">
        <motion.button
          whileTap={{ scale: 0.96 }}
          onClick={handleNext}
          className="w-full py-4 rounded-2xl bg-white text-[#101622] font-semibold text-[16px] flex items-center justify-center gap-2 shadow-[0_0_30px_rgba(255,255,255,0.1)]"
        >
          {currentStep === steps.length - 1 ? 'Get Started' : 'Continue'}
          <ArrowRight className="w-5 h-5" />
        </motion.button>
      </div>
    </div>
  );
}
