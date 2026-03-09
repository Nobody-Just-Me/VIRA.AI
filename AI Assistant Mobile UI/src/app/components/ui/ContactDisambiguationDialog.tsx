import { AnimatePresence, motion } from 'motion/react';
import { X, Phone, MessageSquare, User } from 'lucide-react';

export interface Contact {
  id: string;
  name: string;
  phone: string;
  label: string; // e.g. "Mobile", "Work", "Home"
  avatar?: string;
}

interface ContactDisambiguationDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onSelect: (contact: Contact) => void;
  searchName: string;
  contacts: Contact[];
}

export function ContactDisambiguationDialog({
  isOpen,
  onClose,
  onSelect,
  searchName,
  contacts,
}: ContactDisambiguationDialogProps) {
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
              className="w-full max-w-sm bg-[#1e2a3a] border border-white/10 rounded-3xl p-5 shadow-2xl pointer-events-auto flex flex-col max-h-[80vh]"
            >
              <div className="flex justify-between items-start mb-4">
                <div>
                  <h3 className="text-lg font-semibold text-white tracking-tight">
                    Which {searchName}?
                  </h3>
                  <p className="text-sm text-slate-400 mt-1">
                    Found multiple contacts matching this name.
                  </p>
                </div>
                <button
                  onClick={onClose}
                  className="w-8 h-8 rounded-full bg-white/5 flex items-center justify-center text-slate-400 hover:text-white hover:bg-white/10 transition-colors"
                >
                  <X className="w-4 h-4" />
                </button>
              </div>

              <div className="overflow-y-auto pr-1 -mr-1 space-y-2 pb-2 scroll-smooth" style={{ scrollbarWidth: 'thin' }}>
                {contacts.map((contact) => (
                  <motion.button
                    key={contact.id}
                    whileTap={{ scale: 0.98 }}
                    onClick={() => onSelect(contact)}
                    className="w-full flex items-center justify-between p-3.5 rounded-2xl bg-white/5 border border-white/5 hover:bg-white/10 hover:border-white/10 transition-all text-left group"
                  >
                    <div className="flex items-center gap-3">
                      <div className="w-12 h-12 rounded-full bg-gradient-to-br from-indigo-500/20 to-purple-500/20 border border-indigo-500/30 flex items-center justify-center overflow-hidden">
                        {contact.avatar ? (
                          <img src={contact.avatar} alt={contact.name} className="w-full h-full object-cover" />
                        ) : (
                          <User className="w-5 h-5 text-indigo-400" />
                        )}
                      </div>
                      <div>
                        <p className="text-white font-medium text-[15px] group-hover:text-indigo-300 transition-colors">
                          {contact.name}
                        </p>
                        <div className="flex items-center gap-2 mt-0.5">
                          <span className="text-xs text-slate-400">{contact.phone}</span>
                          <span className="w-1 h-1 rounded-full bg-slate-600" />
                          <span className="text-[10px] uppercase font-medium tracking-wider text-slate-500">
                            {contact.label}
                          </span>
                        </div>
                      </div>
                    </div>
                    
                    <div className="flex gap-1.5 opacity-60 group-hover:opacity-100 transition-opacity">
                      <div className="w-8 h-8 rounded-full bg-blue-500/10 flex items-center justify-center">
                        <MessageSquare className="w-4 h-4 text-blue-400" />
                      </div>
                      <div className="w-8 h-8 rounded-full bg-green-500/10 flex items-center justify-center">
                        <Phone className="w-4 h-4 text-green-400" />
                      </div>
                    </div>
                  </motion.button>
                ))}
              </div>
            </motion.div>
          </div>
        </>
      )}
    </AnimatePresence>
  );
}
