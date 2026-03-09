import { motion, AnimatePresence } from 'motion/react';
import { X, MessageSquare, Plus, Trash2, Clock } from 'lucide-react';
import { useState } from 'react';

interface ChatHistorySidebarProps {
  isOpen: boolean;
  onClose: () => void;
  onSelectChat: (id: string, title: string) => void;
  onNewChat: () => void;
  onClearHistory: () => void;
}

const HISTORY_GROUPS = [
  {
    label: 'Today',
    chats: [
      { id: 'chat_1', title: 'Schedule & Weather updates' },
      { id: 'chat_2', title: 'Coffee order at Kenangan' },
    ],
  },
  {
    label: 'Yesterday',
    chats: [
      { id: 'chat_3', title: 'Meeting notes summary' },
      { id: 'chat_4', title: 'Traffic route to Downtown' },
    ],
  },
  {
    label: 'Previous 7 Days',
    chats: [
      { id: 'chat_5', title: 'Weekend trip planning' },
      { id: 'chat_6', title: 'Focus music playlist' },
      { id: 'chat_7', title: 'Project brainstorming' },
    ],
  },
];

export function ChatHistorySidebar({
  isOpen,
  onClose,
  onSelectChat,
  onNewChat,
  onClearHistory,
}: ChatHistorySidebarProps) {
  const [showClearConfirm, setShowClearConfirm] = useState(false);

  const handleClear = () => {
    onClearHistory();
    setShowClearConfirm(false);
  };

  return (
    <AnimatePresence>
      {isOpen && (
        <>
          {/* Backdrop */}
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            onClick={onClose}
            className="fixed inset-0 bg-[#000000]/60 backdrop-blur-sm z-40"
          />

          {/* Sidebar Panel */}
          <motion.div
            initial={{ x: '-100%' }}
            animate={{ x: 0 }}
            exit={{ x: '-100%' }}
            transition={{ type: 'spring', damping: 25, stiffness: 200 }}
            className="fixed top-0 left-0 bottom-0 w-[280px] bg-[#101622] border-r border-white/5 z-50 flex flex-col shadow-2xl"
          >
            {/* Header */}
            <div className="flex items-center justify-between px-5 py-4 border-b border-white/5">
              <div className="flex items-center gap-2">
                <Clock className="w-5 h-5 text-[#8b5cf6]" />
                <h2 className="text-white font-semibold tracking-tight text-[16px]">History</h2>
              </div>
              <button
                onClick={onClose}
                className="w-8 h-8 rounded-full flex items-center justify-center text-slate-400 hover:bg-white/5 hover:text-white transition-colors"
              >
                <X className="w-5 h-5" />
              </button>
            </div>

            {/* New Chat Button */}
            <div className="p-4">
              <button
                onClick={onNewChat}
                className="w-full flex items-center gap-2 bg-[#8b5cf6]/10 hover:bg-[#8b5cf6]/20 text-[#a855f7] border border-[#8b5cf6]/20 rounded-xl px-4 py-3 transition-colors"
              >
                <Plus className="w-4 h-4" />
                <span className="font-medium text-[14px]">New Conversation</span>
              </button>
            </div>

            {/* History List */}
            <div className="flex-1 overflow-y-auto px-3 pb-4 space-y-6" style={{ scrollbarWidth: 'none' }}>
              {HISTORY_GROUPS.map((group, groupIdx) => (
                <div key={groupIdx}>
                  <h3 className="text-[12px] font-semibold text-slate-500 uppercase tracking-wider mb-2 px-2">
                    {group.label}
                  </h3>
                  <div className="space-y-1">
                    {group.chats.map((chat) => (
                      <button
                        key={chat.id}
                        onClick={() => onSelectChat(chat.id, chat.title)}
                        className="w-full flex items-start gap-3 px-3 py-2.5 rounded-xl hover:bg-white/5 text-left transition-colors group"
                      >
                        <MessageSquare className="w-4 h-4 text-slate-400 mt-0.5 group-hover:text-[#8b5cf6] transition-colors" />
                        <span className="text-[14px] text-slate-300 group-hover:text-white leading-tight truncate">
                          {chat.title}
                        </span>
                      </button>
                    ))}
                  </div>
                </div>
              ))}
            </div>

            {/* Footer / Clear History */}
            <div className="p-4 border-t border-white/5">
              {showClearConfirm ? (
                <motion.div
                  initial={{ opacity: 0, y: 10 }}
                  animate={{ opacity: 1, y: 0 }}
                  className="bg-red-500/10 border border-red-500/20 rounded-xl p-3"
                >
                  <p className="text-[13px] text-red-200 mb-3 text-center">Clear all chat history?</p>
                  <div className="flex gap-2">
                    <button
                      onClick={handleClear}
                      className="flex-1 py-1.5 bg-red-500 hover:bg-red-600 text-white rounded-lg text-[13px] font-medium transition-colors"
                    >
                      Yes, clear
                    </button>
                    <button
                      onClick={() => setShowClearConfirm(false)}
                      className="flex-1 py-1.5 bg-white/5 hover:bg-white/10 text-slate-300 rounded-lg text-[13px] font-medium transition-colors"
                    >
                      Cancel
                    </button>
                  </div>
                </motion.div>
              ) : (
                <button
                  onClick={() => setShowClearConfirm(true)}
                  className="w-full flex items-center justify-center gap-2 py-3 text-slate-400 hover:text-red-400 hover:bg-red-400/10 rounded-xl transition-colors"
                >
                  <Trash2 className="w-4 h-4" />
                  <span className="text-[14px] font-medium">Clear History</span>
                </button>
              )}
            </div>
          </motion.div>
        </>
      )}
    </AnimatePresence>
  );
}
