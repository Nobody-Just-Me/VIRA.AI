import { Toaster, toast } from 'sonner';
import { AlertTriangle, XCircle, CheckCircle2, Info } from 'lucide-react';
import { motion } from 'motion/react';

export function AppToaster() {
  return (
    <Toaster
      position="top-center"
      toastOptions={{
        className: 'bg-[#1e2a3a] border-white/10 text-white shadow-2xl rounded-2xl p-4',
        style: {
          backdropFilter: 'blur(12px)',
        },
      }}
    />
  );
}

interface ToastOptions {
  title: string;
  description?: string;
  duration?: number;
}

export const ErrorSnackbar = {
  show: ({ title, description, duration = 4000 }: ToastOptions) => {
    toast.custom(
      (t) => (
        <motion.div
          initial={{ opacity: 0, y: -20, scale: 0.95 }}
          animate={{ opacity: 1, y: 0, scale: 1 }}
          exit={{ opacity: 0, scale: 0.95 }}
          className="w-[340px] bg-[#1e2a3a]/95 backdrop-blur-xl border border-red-500/20 rounded-2xl shadow-[0_8px_32px_-8px_rgba(239,68,68,0.25)] p-4 flex gap-3 relative overflow-hidden"
        >
          <div className="absolute left-0 top-0 bottom-0 w-1 bg-red-500" />
          <div className="w-8 h-8 rounded-full bg-red-500/10 flex items-center justify-center flex-shrink-0 mt-0.5">
            <AlertTriangle className="w-4 h-4 text-red-400" />
          </div>
          <div className="flex-1 min-w-0">
            <h4 className="text-[14px] font-semibold text-white tracking-tight leading-snug">
              {title}
            </h4>
            {description && (
              <p className="text-[13px] text-slate-300 mt-1 leading-relaxed">
                {description}
              </p>
            )}
          </div>
          <button
            onClick={() => toast.dismiss(t)}
            className="w-6 h-6 flex items-center justify-center rounded-full bg-white/5 hover:bg-white/10 text-slate-400 transition-colors flex-shrink-0"
          >
            <XCircle className="w-4 h-4" />
          </button>
        </motion.div>
      ),
      { duration }
    );
  },
  
  success: ({ title, description, duration = 3000 }: ToastOptions) => {
    toast.custom(
      (t) => (
        <motion.div
          initial={{ opacity: 0, y: -20, scale: 0.95 }}
          animate={{ opacity: 1, y: 0, scale: 1 }}
          exit={{ opacity: 0, scale: 0.95 }}
          className="w-[340px] bg-[#1e2a3a]/95 backdrop-blur-xl border border-green-500/20 rounded-2xl shadow-[0_8px_32px_-8px_rgba(34,197,94,0.2)] p-4 flex gap-3 relative overflow-hidden"
        >
          <div className="absolute left-0 top-0 bottom-0 w-1 bg-green-500" />
          <div className="w-8 h-8 rounded-full bg-green-500/10 flex items-center justify-center flex-shrink-0 mt-0.5">
            <CheckCircle2 className="w-4 h-4 text-green-400" />
          </div>
          <div className="flex-1 min-w-0">
            <h4 className="text-[14px] font-semibold text-white tracking-tight leading-snug">
              {title}
            </h4>
            {description && (
              <p className="text-[13px] text-slate-300 mt-1 leading-relaxed">
                {description}
              </p>
            )}
          </div>
          <button
            onClick={() => toast.dismiss(t)}
            className="w-6 h-6 flex items-center justify-center rounded-full bg-white/5 hover:bg-white/10 text-slate-400 transition-colors flex-shrink-0"
          >
            <XCircle className="w-4 h-4" />
          </button>
        </motion.div>
      ),
      { duration }
    );
  },

  info: ({ title, description, duration = 3000 }: ToastOptions) => {
    toast.custom(
      (t) => (
        <motion.div
          initial={{ opacity: 0, y: -20, scale: 0.95 }}
          animate={{ opacity: 1, y: 0, scale: 1 }}
          exit={{ opacity: 0, scale: 0.95 }}
          className="w-[340px] bg-[#1e2a3a]/95 backdrop-blur-xl border border-blue-500/20 rounded-2xl shadow-[0_8px_32px_-8px_rgba(59,130,246,0.2)] p-4 flex gap-3 relative overflow-hidden"
        >
          <div className="absolute left-0 top-0 bottom-0 w-1 bg-blue-500" />
          <div className="w-8 h-8 rounded-full bg-blue-500/10 flex items-center justify-center flex-shrink-0 mt-0.5">
            <Info className="w-4 h-4 text-blue-400" />
          </div>
          <div className="flex-1 min-w-0">
            <h4 className="text-[14px] font-semibold text-white tracking-tight leading-snug">
              {title}
            </h4>
            {description && (
              <p className="text-[13px] text-slate-300 mt-1 leading-relaxed">
                {description}
              </p>
            )}
          </div>
          <button
            onClick={() => toast.dismiss(t)}
            className="w-6 h-6 flex items-center justify-center rounded-full bg-white/5 hover:bg-white/10 text-slate-400 transition-colors flex-shrink-0"
          >
            <XCircle className="w-4 h-4" />
          </button>
        </motion.div>
      ),
      { duration }
    );
  },
};
