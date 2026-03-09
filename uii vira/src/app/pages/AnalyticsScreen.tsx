import { motion } from 'motion/react';
import { ArrowLeft, TrendingUp, CheckCircle, Clock, Zap, Target } from 'lucide-react';
import { useNavigate } from 'react-router';

export default function AnalyticsScreen() {
  const navigate = useNavigate();

  return (
    <div className="min-h-screen bg-[#101622] text-white font-sans pb-10">
      {/* Header */}
      <header className="sticky top-0 z-20 flex items-center justify-between px-5 py-4 bg-[#101622]/80 backdrop-blur-md border-b border-white/5">
        <motion.button
          whileTap={{ scale: 0.92 }}
          onClick={() => navigate(-1)}
          className="w-10 h-10 rounded-full flex items-center justify-center bg-white/5 hover:bg-white/10 transition-colors"
        >
          <ArrowLeft className="w-5 h-5 text-slate-300" />
        </motion.button>
        <h1 className="text-[17px] font-semibold tracking-tight">Productivity</h1>
        <div className="w-10" /> {/* Spacer */}
      </header>

      <main className="px-5 pt-6 space-y-6">
        {/* Weekly Overview */}
        <div>
          <h2 className="text-sm font-semibold text-slate-400 uppercase tracking-wider mb-4">Weekly Overview</h2>
          <div className="grid grid-cols-2 gap-3">
            <div className="bg-white/5 border border-white/10 rounded-3xl p-5 flex flex-col items-center text-center">
              <div className="w-10 h-10 rounded-full bg-green-500/20 text-green-400 flex items-center justify-center mb-3">
                <CheckCircle className="w-5 h-5" />
              </div>
              <p className="text-3xl font-bold mb-1">34</p>
              <p className="text-xs text-slate-400">Tasks Completed</p>
              <div className="flex items-center gap-1 mt-2 text-[10px] text-green-400 bg-green-400/10 px-2 py-0.5 rounded-full">
                <TrendingUp className="w-3 h-3" /> +12%
              </div>
            </div>
            
            <div className="bg-white/5 border border-white/10 rounded-3xl p-5 flex flex-col items-center text-center">
              <div className="w-10 h-10 rounded-full bg-blue-500/20 text-blue-400 flex items-center justify-center mb-3">
                <Clock className="w-5 h-5" />
              </div>
              <p className="text-3xl font-bold mb-1">4.2h</p>
              <p className="text-xs text-slate-400">Time Saved</p>
              <div className="flex items-center gap-1 mt-2 text-[10px] text-blue-400 bg-blue-400/10 px-2 py-0.5 rounded-full">
                <TrendingUp className="w-3 h-3" /> +5%
              </div>
            </div>
          </div>
        </div>

        {/* Focus Score */}
        <div className="bg-gradient-to-br from-indigo-500/20 to-purple-500/10 border border-indigo-500/20 rounded-3xl p-6 relative overflow-hidden">
          <div className="absolute top-0 right-0 w-32 h-32 bg-indigo-500/20 blur-[40px] rounded-full pointer-events-none" />
          
          <div className="flex items-center gap-3 mb-4 relative z-10">
            <div className="w-8 h-8 rounded-full bg-indigo-500/30 flex items-center justify-center text-indigo-300">
              <Zap className="w-4 h-4" />
            </div>
            <h3 className="font-medium">Focus Score</h3>
          </div>
          
          <div className="flex items-end gap-4 relative z-10">
            <span className="text-5xl font-bold tracking-tight">85</span>
            <span className="text-slate-400 mb-1.5">/ 100</span>
          </div>
          
          <div className="mt-5 w-full bg-white/10 rounded-full h-2">
            <motion.div 
              initial={{ width: 0 }}
              animate={{ width: '85%' }}
              transition={{ duration: 1, ease: 'easeOut' }}
              className="h-full rounded-full bg-gradient-to-r from-indigo-400 to-purple-400"
            />
          </div>
          <p className="text-xs text-indigo-200 mt-3 relative z-10">Your focus is 15% better than last week. Great job!</p>
        </div>

        {/* AI Assistance Breakdown */}
        <div>
          <h2 className="text-sm font-semibold text-slate-400 uppercase tracking-wider mb-4">Vira Assistance</h2>
          <div className="bg-white/5 border border-white/10 rounded-3xl p-5 space-y-4">
            {[
              { label: 'Drafting Emails', value: 45, color: 'bg-blue-400' },
              { label: 'Scheduling', value: 30, color: 'bg-purple-400' },
              { label: 'Research', value: 15, color: 'bg-emerald-400' },
              { label: 'Other', value: 10, color: 'bg-slate-400' },
            ].map((stat, i) => (
              <div key={i} className="flex items-center gap-3">
                <div className="w-8 text-right text-xs font-medium text-slate-400">{stat.value}%</div>
                <div className="flex-1 h-2.5 bg-white/10 rounded-full overflow-hidden">
                  <motion.div 
                    initial={{ width: 0 }}
                    animate={{ width: `${stat.value}%` }}
                    transition={{ duration: 0.8, delay: i * 0.1 }}
                    className={`h-full ${stat.color}`}
                  />
                </div>
                <div className="w-24 text-xs text-slate-300">{stat.label}</div>
              </div>
            ))}
          </div>
        </div>

        {/* Daily Goal */}
        <div className="bg-white/5 border border-white/10 rounded-3xl p-5 flex items-center justify-between">
          <div className="flex items-center gap-4">
            <div className="w-12 h-12 rounded-full bg-amber-500/20 text-amber-400 flex items-center justify-center">
              <Target className="w-6 h-6" />
            </div>
            <div>
              <p className="font-medium text-[15px]">Daily Reading</p>
              <p className="text-xs text-slate-400">20 / 30 mins</p>
            </div>
          </div>
          <button className="px-4 py-2 bg-white/10 hover:bg-white/15 rounded-xl text-xs font-medium transition-colors">
            Log Time
          </button>
        </div>

      </main>
    </div>
  );
}
