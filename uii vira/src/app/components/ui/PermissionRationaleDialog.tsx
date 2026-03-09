import { AnimatePresence, motion } from 'motion/react';
import { AlertCircle, Mic, Camera, MapPin, X } from 'lucide-react';

interface PermissionRationaleDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onGrant: () => void;
  permissionType: 'microphone' | 'camera' | 'location';
}

export function PermissionRationaleDialog({
  isOpen,
  onClose,
  onGrant,
  permissionType,
}: PermissionRationaleDialogProps) {
  const content = {
    microphone: {
      icon: Mic,
      title: 'Microphone Access',
      description:
        'Vira needs access to your microphone so you can use voice commands to chat, set reminders, and ask questions hands-free.',
      color: 'text-purple-500',
      bg: 'bg-purple-500/10',
      border: 'border-purple-500/20',
    },
    camera: {
      icon: Camera,
      title: 'Camera Access',
      description:
        'Vira needs access to your camera to analyze objects, translate text from images, and provide visual assistance.',
      color: 'text-blue-500',
      bg: 'bg-blue-500/10',
      border: 'border-blue-500/20',
    },
    location: {
      icon: MapPin,
      title: 'Location Access',
      description:
        'Vira needs your location to provide accurate weather forecasts, traffic updates, and local recommendations.',
      color: 'text-green-500',
      bg: 'bg-green-500/10',
      border: 'border-green-500/20',
    },
  }[permissionType];

  const Icon = content.icon;

  return (
    <AnimatePresence>
      {isOpen && (
        <>
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            onClick={onClose}
            className="fixed inset-0 bg-[#101622]/80 backdrop-blur-sm z-50"
          />
          <div className="fixed inset-0 flex items-center justify-center z-50 px-4 pointer-events-none">
            <motion.div
              initial={{ opacity: 0, scale: 0.95, y: 10 }}
              animate={{ opacity: 1, scale: 1, y: 0 }}
              exit={{ opacity: 0, scale: 0.95, y: 10 }}
              transition={{ type: 'spring', damping: 25, stiffness: 300 }}
              className="w-full max-w-sm bg-[#1e2a3a] border border-white/10 rounded-3xl p-6 shadow-2xl pointer-events-auto relative overflow-hidden"
            >
              {/* Ambient Background Glow */}
              <div className="absolute -top-20 -right-20 w-40 h-40 bg-purple-500/20 blur-[50px] rounded-full pointer-events-none" />

              <button
                onClick={onClose}
                className="absolute top-4 right-4 text-slate-400 hover:text-white transition-colors"
              >
                <X className="w-5 h-5" />
              </button>

              <div className="flex flex-col items-center text-center mt-2">
                <div
                  className={`w-16 h-16 rounded-2xl flex items-center justify-center mb-5 ${content.bg} ${content.border} border`}
                >
                  <Icon className={`w-8 h-8 ${content.color}`} />
                </div>

                <h3 className="text-xl font-semibold text-white mb-2 tracking-tight">
                  {content.title}
                </h3>
                
                <p className="text-[15px] text-slate-300 leading-relaxed mb-6">
                  {content.description}
                </p>

                <div className="flex items-start gap-3 bg-white/5 border border-white/5 rounded-xl p-3 mb-6 text-left w-full">
                  <AlertCircle className="w-5 h-5 text-blue-400 flex-shrink-0 mt-0.5" />
                  <p className="text-xs text-slate-400 leading-relaxed">
                    You can change this anytime in your device settings. Vira values your privacy and only uses this permission when necessary.
                  </p>
                </div>

                <div className="flex flex-col gap-3 w-full">
                  <motion.button
                    whileTap={{ scale: 0.96 }}
                    onClick={onGrant}
                    className="w-full py-3.5 bg-[#256af4] hover:bg-[#1d58d6] text-white rounded-xl font-medium text-[15px] transition-colors shadow-[0_4px_14px_-2px_rgba(37,106,244,0.4)]"
                  >
                    Allow Access
                  </motion.button>
                  <motion.button
                    whileTap={{ scale: 0.96 }}
                    onClick={onClose}
                    className="w-full py-3.5 bg-white/5 hover:bg-white/10 text-slate-300 rounded-xl font-medium text-[15px] transition-colors"
                  >
                    Not Now
                  </motion.button>
                </div>
              </div>
            </motion.div>
          </div>
        </>
      )}
    </AnimatePresence>
  );
}
