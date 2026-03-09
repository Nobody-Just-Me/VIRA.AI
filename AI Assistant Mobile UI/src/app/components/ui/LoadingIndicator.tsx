import { motion } from 'motion/react';
import { Sparkles } from 'lucide-react';

export function LoadingIndicator({ message = "Vira is thinking..." }: { message?: string }) {
  return (
    <motion.div
      initial={{ opacity: 0, y: 10, scale: 0.95 }}
      animate={{ opacity: 1, y: 0, scale: 1 }}
      exit={{ opacity: 0, y: 5 }}
      className="flex w-full justify-start my-2"
    >
      <div className="max-w-[85%] flex flex-col items-start gap-1.5">
        <div className="flex items-center gap-2 ml-1">
          <div className="flex items-center justify-center w-5 h-5 rounded-full bg-gradient-to-tr from-[#6366f1] to-[#9333ea] shadow-[0_0_10px_rgba(99,102,241,0.4)]">
            <Sparkles className="w-3 h-3 text-white" />
          </div>
          <span className="text-[11px] font-medium text-slate-400 uppercase tracking-widest">{message}</span>
        </div>
        
        <div className="px-5 py-4 bg-white/5 backdrop-blur-md border border-white/10 rounded-t-2xl rounded-br-2xl rounded-bl-[2px] flex items-center gap-2">
          {/* Animated Sine Wave */}
          <div className="flex items-center gap-1">
            {[0, 1, 2, 3].map((i) => (
              <motion.div
                key={i}
                className="w-1.5 rounded-full bg-gradient-to-t from-[#25d1f4] to-[#a855f7]"
                animate={{
                  height: [6, 16, 6],
                  opacity: [0.4, 1, 0.4]
                }}
                transition={{
                  duration: 1,
                  repeat: Infinity,
                  delay: i * 0.15,
                  ease: "easeInOut"
                }}
              />
            ))}
          </div>
        </div>
      </div>
    </motion.div>
  );
}
