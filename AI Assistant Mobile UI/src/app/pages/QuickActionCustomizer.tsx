import { useState } from 'react';
import { motion } from 'motion/react';
import { ArrowLeft, GripVertical, Plus, Sun, Newspaper, Bell, Car, Coffee, Music, X, Save } from 'lucide-react';
import { useNavigate } from 'react-router';
import { ErrorSnackbar } from '../components/ui/ErrorSnackbar';

interface ActionItem {
  id: string;
  label: string;
  icon: any;
  color: string;
}

const AVAILABLE_ACTIONS = [
  { id: '1', label: 'Weather', icon: Sun, color: 'text-yellow-400' },
  { id: '2', label: 'News', icon: Newspaper, color: 'text-blue-400' },
  { id: '3', label: 'Reminders', icon: Bell, color: 'text-red-400' },
  { id: '4', label: 'Traffic', icon: Car, color: 'text-green-400' },
  { id: '5', label: 'Coffee', icon: Coffee, color: 'text-amber-500' },
  { id: '6', label: 'Music', icon: Music, color: 'text-purple-400' },
];

export default function QuickActionCustomizer() {
  const navigate = useNavigate();
  const [activeActions, setActiveActions] = useState<ActionItem[]>(AVAILABLE_ACTIONS.slice(0, 4));
  const [availableActions, setAvailableActions] = useState<ActionItem[]>(AVAILABLE_ACTIONS.slice(4));

  const handleRemove = (action: ActionItem) => {
    setActiveActions(prev => prev.filter(a => a.id !== action.id));
    setAvailableActions(prev => [...prev, action]);
  };

  const handleAdd = (action: ActionItem) => {
    if (activeActions.length >= 6) {
      ErrorSnackbar.show({
        title: "Limit Reached",
        description: "You can only have up to 6 quick actions.",
      });
      return;
    }
    setAvailableActions(prev => prev.filter(a => a.id !== action.id));
    setActiveActions(prev => [...prev, action]);
  };

  const handleSave = () => {
    ErrorSnackbar.success({
      title: "Saved",
      description: "Your quick actions have been updated.",
    });
    setTimeout(() => navigate(-1), 1000);
  };

  return (
    <div className="min-h-screen bg-[#101622] text-white font-sans flex flex-col">
      {/* Header */}
      <header className="sticky top-0 z-20 flex items-center justify-between px-5 py-4 bg-[#101622]/80 backdrop-blur-md border-b border-white/5">
        <motion.button
          whileTap={{ scale: 0.92 }}
          onClick={() => navigate(-1)}
          className="w-10 h-10 rounded-full flex items-center justify-center bg-white/5 hover:bg-white/10 transition-colors"
        >
          <ArrowLeft className="w-5 h-5 text-slate-300" />
        </motion.button>
        <h1 className="text-[17px] font-semibold tracking-tight">Customize Actions</h1>
        <motion.button
          whileTap={{ scale: 0.92 }}
          onClick={handleSave}
          className="w-10 h-10 rounded-full flex items-center justify-center bg-[#8b5cf6] hover:bg-[#7c3aed] transition-colors"
        >
          <Save className="w-4 h-4 text-white" />
        </motion.button>
      </header>

      <main className="flex-1 overflow-y-auto px-5 pt-6 pb-12 space-y-8">
        <div>
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-sm font-semibold text-slate-400 uppercase tracking-wider">Active Actions</h2>
            <span className="text-xs text-slate-500">{activeActions.length}/6</span>
          </div>
          <div className="space-y-2">
            {activeActions.map((action) => (
              <motion.div
                key={action.id}
                layoutId={action.id}
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, scale: 0.95 }}
                className="flex items-center justify-between bg-white/5 border border-white/10 rounded-2xl p-3.5"
              >
                <div className="flex items-center gap-3">
                  <GripVertical className="w-4 h-4 text-slate-500 cursor-grab" />
                  <div className={`w-8 h-8 rounded-full bg-white/5 flex items-center justify-center ${action.color}`}>
                    <action.icon className="w-4 h-4" />
                  </div>
                  <span className="font-medium text-[15px]">{action.label}</span>
                </div>
                <button
                  onClick={() => handleRemove(action)}
                  className="w-8 h-8 rounded-full flex items-center justify-center bg-white/5 text-slate-400 hover:text-red-400 hover:bg-red-400/10 transition-colors"
                >
                  <X className="w-4 h-4" />
                </button>
              </motion.div>
            ))}
            {activeActions.length === 0 && (
              <div className="text-center py-8 text-slate-500 text-sm bg-white/5 rounded-2xl border border-dashed border-white/10">
                No active actions. Add some from below.
              </div>
            )}
          </div>
        </div>

        <div>
          <h2 className="text-sm font-semibold text-slate-400 uppercase tracking-wider mb-4">Available Actions</h2>
          <div className="space-y-2">
            {availableActions.map((action) => (
              <motion.div
                key={action.id}
                layoutId={action.id}
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, scale: 0.95 }}
                className="flex items-center justify-between bg-white/5 border border-white/10 rounded-2xl p-3.5 opacity-60 hover:opacity-100 transition-opacity"
              >
                <div className="flex items-center gap-3">
                  <div className={`w-8 h-8 rounded-full bg-white/5 flex items-center justify-center ${action.color}`}>
                    <action.icon className="w-4 h-4" />
                  </div>
                  <span className="font-medium text-[15px]">{action.label}</span>
                </div>
                <button
                  onClick={() => handleAdd(action)}
                  className="w-8 h-8 rounded-full flex items-center justify-center bg-[#8b5cf6]/20 text-[#a855f7] hover:bg-[#8b5cf6]/40 transition-colors"
                >
                  <Plus className="w-4 h-4" />
                </button>
              </motion.div>
            ))}
          </div>
        </div>
      </main>
    </div>
  );
}
