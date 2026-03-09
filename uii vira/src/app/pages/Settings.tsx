import { useState } from 'react';
import {
  ArrowLeft,
  Check,
  Moon,
  Sun,
  Globe,
  Zap,
  HelpCircle,
  LogOut,
  Volume2,
  Key,
  ChevronRight,
  MessageSquare,
  BrainCircuit,
  Shield,
  Languages,
  Bell,
  Trash2,
  Star,
} from 'lucide-react';
import { useNavigate } from 'react-router';
import { motion, AnimatePresence } from 'motion/react';

// ─── Sub-components ───────────────────────────────────────────────────────────

function ViraProfileHeader() {
  return (
    <div
      className="relative overflow-hidden rounded-3xl p-5 mx-4 mt-4"
      style={{
        background: 'linear-gradient(135deg, #1e1b4b 0%, #2d1b69 50%, #1a1535 100%)',
        border: '1px solid rgba(99,102,241,0.2)',
        boxShadow: '0 8px 32px -4px rgba(99,102,241,0.2)',
      }}
    >
      {/* Ambient glow */}
      <div
        className="absolute top-0 right-0 w-32 h-32 rounded-full pointer-events-none"
        style={{
          background: 'rgba(147,51,234,0.2)',
          filter: 'blur(30px)',
          transform: 'translate(30%, -30%)',
        }}
      />
      <div
        className="absolute bottom-0 left-0 w-24 h-24 rounded-full pointer-events-none"
        style={{
          background: 'rgba(99,102,241,0.2)',
          filter: 'blur(25px)',
          transform: 'translate(-20%, 20%)',
        }}
      />

      <div className="flex items-center gap-4 relative z-10">
        {/* Vira Avatar */}
        <div className="relative">
          <div
            className="w-16 h-16 rounded-2xl flex items-center justify-center"
            style={{
              background: 'linear-gradient(135deg, #6366f1 0%, #9333ea 100%)',
              boxShadow: '0 0 20px rgba(99,102,241,0.6)',
            }}
          >
            <span
              className="text-white select-none"
              style={{ fontSize: 28, fontWeight: 800, letterSpacing: '-1.5px' }}
            >
              V
            </span>
          </div>
          <div
            className="absolute -bottom-1 -right-1 w-5 h-5 rounded-full flex items-center justify-center"
            style={{ background: '#22c55e', border: '2px solid #1e1b4b' }}
          >
            <div className="w-2 h-2 rounded-full bg-white" />
          </div>
        </div>

        <div className="flex-1">
          <div className="flex items-center gap-2">
            <h2
              className="text-white font-bold"
              style={{ fontSize: 20, letterSpacing: '-0.5px' }}
            >
              Vira
            </h2>
            <div
              className="px-2 py-0.5 rounded-full text-white"
              style={{
                background: 'rgba(99,102,241,0.3)',
                border: '1px solid rgba(99,102,241,0.4)',
                fontSize: 10,
                fontWeight: 600,
                letterSpacing: '0.05em',
              }}
            >
              PRO
            </div>
          </div>
          <p style={{ color: 'rgba(255,255,255,0.5)', fontSize: 13 }}>
            Your Personal AI Assistant
          </p>
          <div className="flex items-center gap-1 mt-1">
            {[1, 2, 3, 4, 5].map((s) => (
              <Star key={s} className="w-3 h-3 fill-yellow-400 text-yellow-400" />
            ))}
            <span style={{ color: 'rgba(255,255,255,0.4)', fontSize: 11, marginLeft: 4 }}>
              v2.4.0
            </span>
          </div>
        </div>

        <div className="text-right">
          <div
            className="px-3 py-1.5 rounded-full text-white"
            style={{
              background: 'rgba(255,255,255,0.08)',
              border: '1px solid rgba(255,255,255,0.12)',
              fontSize: 12,
              fontWeight: 500,
            }}
          >
            Active
          </div>
        </div>
      </div>

      {/* Stats row */}
      <div
        className="grid grid-cols-3 gap-3 mt-4 pt-4 relative z-10"
        style={{ borderTop: '1px solid rgba(255,255,255,0.08)' }}
      >
        {[
          { label: 'Conversations', value: '1,248' },
          { label: 'Questions', value: '5,891' },
          { label: 'Days Active', value: '42' },
        ].map((stat) => (
          <div key={stat.label} className="text-center">
            <p className="text-white font-bold" style={{ fontSize: 16 }}>{stat.value}</p>
            <p style={{ color: 'rgba(255,255,255,0.4)', fontSize: 10 }}>{stat.label}</p>
          </div>
        ))}
      </div>
    </div>
  );
}

interface ToggleRowProps {
  icon: React.ReactNode;
  iconBg: string;
  title: string;
  subtitle: string;
  checked: boolean;
  onChange: (v: boolean) => void;
  isFirst?: boolean;
}

function ToggleRow({ icon, iconBg, title, subtitle, checked, onChange, isFirst }: ToggleRowProps) {
  return (
    <div
      className="flex items-center justify-between px-4 py-3.5"
      style={!isFirst ? { borderTop: '1px solid #f1f5f9' } : {}}
    >
      <div className="flex items-center gap-3">
        <div
          className="w-8 h-8 rounded-full flex items-center justify-center flex-shrink-0"
          style={{ background: iconBg }}
        >
          {icon}
        </div>
        <div>
          <p className="text-[14px] font-medium text-slate-900">{title}</p>
          <p className="text-[12px] text-slate-500">{subtitle}</p>
        </div>
      </div>
      <button
        role="switch"
        aria-checked={checked}
        onClick={() => onChange(!checked)}
        className="relative flex-shrink-0 transition-all duration-300"
        style={{ width: 44, height: 24 }}
      >
        <div
          className="absolute inset-0 rounded-full transition-all duration-300"
          style={{ background: checked ? '#2094f3' : '#e2e8f0' }}
        />
        <motion.div
          className="absolute top-[2px] w-5 h-5 rounded-full bg-white shadow-sm"
          animate={{ x: checked ? 22 : 2 }}
          transition={{ type: 'spring', stiffness: 500, damping: 30 }}
        />
      </button>
    </div>
  );
}

interface LinkRowProps {
  icon: React.ReactNode;
  title: string;
  subtitle?: string;
  onClick?: () => void;
  danger?: boolean;
  isFirst?: boolean;
}

function LinkRow({ icon, title, subtitle, onClick, danger, isFirst }: LinkRowProps) {
  return (
    <button
      onClick={onClick}
      className="w-full flex items-center justify-between px-4 py-3.5 hover:bg-slate-50/80 active:bg-slate-100 transition-colors text-left"
      style={!isFirst ? { borderTop: '1px solid #f1f5f9' } : {}}
    >
      <div className="flex items-center gap-3">
        <div className="flex-shrink-0">{icon}</div>
        <div>
          <p
            className="text-[14px] font-medium"
            style={{ color: danger ? '#ef4444' : '#0f172a' }}
          >
            {title}
          </p>
          {subtitle && <p className="text-[12px] text-slate-500">{subtitle}</p>}
        </div>
      </div>
      {!danger && <ChevronRight className="w-4 h-4 text-slate-300" />}
    </button>
  );
}

interface SelectRowProps {
  icon: React.ReactNode;
  iconBg: string;
  title: string;
  value: string;
  options: string[];
  onChange: (v: string) => void;
  isFirst?: boolean;
}

function SelectRow({ icon, iconBg, title, value, options, onChange, isFirst }: SelectRowProps) {
  const [open, setOpen] = useState(false);

  return (
    <div
      className="px-4 py-3.5 relative"
      style={!isFirst ? { borderTop: '1px solid #f1f5f9' } : {}}
    >
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <div
            className="w-8 h-8 rounded-full flex items-center justify-center flex-shrink-0"
            style={{ background: iconBg }}
          >
            {icon}
          </div>
          <p className="text-[14px] font-medium text-slate-900">{title}</p>
        </div>
        <button
          onClick={() => setOpen(!open)}
          className="flex items-center gap-1.5 px-3 py-1.5 rounded-full bg-slate-100 hover:bg-slate-200 transition-colors"
        >
          <span className="text-[13px] text-slate-600 font-medium">{value}</span>
          <motion.div animate={{ rotate: open ? 90 : 0 }} transition={{ duration: 0.2 }}>
            <ChevronRight className="w-3.5 h-3.5 text-slate-400" />
          </motion.div>
        </button>
      </div>

      <AnimatePresence>
        {open && (
          <motion.div
            initial={{ opacity: 0, y: -8, height: 0 }}
            animate={{ opacity: 1, y: 0, height: 'auto' }}
            exit={{ opacity: 0, y: -8, height: 0 }}
            className="mt-3 overflow-hidden"
          >
            <div className="flex flex-wrap gap-2 pl-11">
              {options.map((opt) => (
                <button
                  key={opt}
                  onClick={() => { onChange(opt); setOpen(false); }}
                  className="px-3 py-1 rounded-full text-[13px] font-medium transition-colors"
                  style={{
                    background: opt === value ? '#2094f3' : '#f1f5f9',
                    color: opt === value ? 'white' : '#475569',
                  }}
                >
                  {opt}
                </button>
              ))}
            </div>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
}

function SectionHeader({ title }: { title: string }) {
  return (
    <div className="px-4 mb-2">
      <h3
        className="text-[11px] font-semibold tracking-wider uppercase"
        style={{ color: '#64748b' }}
      >
        {title}
      </h3>
    </div>
  );
}

function Card({ children }: { children: React.ReactNode }) {
  return (
    <div
      className="bg-white rounded-2xl overflow-hidden mx-4"
      style={{
        border: '1px solid #f1f5f9',
        boxShadow: '0 1px 3px rgba(0,0,0,0.05)',
      }}
    >
      {children}
    </div>
  );
}

// ─── Main ─────────────────────────────────────────────────────────────────────

export default function Settings() {
  const navigate = useNavigate();

  // API
  const [apiKey, setApiKey] = useState('');
  const [showKey, setShowKey] = useState(false);
  const [isSaved, setIsSaved] = useState(false);

  // Preferences
  const [voiceOutput, setVoiceOutput] = useState(true);
  const [darkMode, setDarkMode] = useState(false);
  const [webBrowsing, setWebBrowsing] = useState(true);
  const [haptics, setHaptics] = useState(true);
  const [notifications, setNotifications] = useState(true);
  const [memoryMode, setMemoryMode] = useState(true);
  const [privacyMode, setPrivacyMode] = useState(false);

  // Selections
  const [language, setLanguage] = useState('English');
  const [model, setModel] = useState('Gemini Pro');
  const [responseStyle, setResponseStyle] = useState('Balanced');

  const [showClearConfirm, setShowClearConfirm] = useState(false);

  const handleSave = () => {
    setIsSaved(true);
    setTimeout(() => setIsSaved(false), 2000);
  };

  return (
    <div className="min-h-screen font-sans" style={{ background: '#F5F7F8' }}>

      {/* Top App Bar */}
      <header
        className="sticky top-0 z-20 flex items-center px-4 h-14"
        style={{
          background: 'rgba(245,247,248,0.92)',
          backdropFilter: 'blur(12px)',
          borderBottom: '1px solid #e2e8f0',
        }}
      >
        <motion.button
          whileTap={{ scale: 0.92 }}
          onClick={() => navigate(-1)}
          className="w-9 h-9 rounded-full flex items-center justify-center"
          style={{ color: '#475569' }}
        >
          <ArrowLeft className="w-5 h-5" />
        </motion.button>
        <h1
          className="absolute left-1/2 -translate-x-1/2 font-semibold"
          style={{ fontSize: 17, color: '#0f172a' }}
        >
          Settings
        </h1>
      </header>

      {/* Scrollable Main */}
      <main className="pb-10 space-y-6 pt-2">

        {/* Vira Profile */}
        <ViraProfileHeader />

        {/* API Configuration */}
        <div className="space-y-2">
          <SectionHeader title="API Configuration" />
          <Card>
            <div className="p-4 space-y-4">
              <div className="space-y-2">
                <label className="text-[14px] font-medium text-slate-700">Gemini API Key</label>
                <div className="relative">
                  <div className="absolute left-3 top-1/2 -translate-y-1/2">
                    <Key className="w-4 h-4 text-slate-400" />
                  </div>
                  <input
                    type={showKey ? 'text' : 'password'}
                    placeholder="sk-........................"
                    value={apiKey}
                    onChange={(e) => setApiKey(e.target.value)}
                    className="w-full h-11 pl-10 pr-20 rounded-xl bg-slate-50 border border-slate-200 text-slate-900 text-[14px] focus:outline-none focus:ring-2 focus:ring-blue-500/30 focus:border-blue-400 transition-all"
                    style={{ fontFamily: apiKey ? 'monospace' : 'inherit' }}
                  />
                  <button
                    onClick={() => setShowKey(!showKey)}
                    className="absolute right-3 top-1/2 -translate-y-1/2 text-[12px] text-blue-500 font-medium hover:text-blue-600 transition-colors"
                  >
                    {showKey ? 'Hide' : 'Show'}
                  </button>
                </div>
                <p className="text-[12px] text-slate-400">Your key is stored securely on your device.</p>
              </div>

              <SelectRow
                icon={<BrainCircuit className="w-4 h-4 text-purple-600" />}
                iconBg="#f3e8ff"
                title="AI Model"
                value={model}
                options={['Gemini Flash', 'Gemini Pro', 'Gemini Ultra']}
                onChange={setModel}
                isFirst
              />

              <motion.button
                whileTap={{ scale: 0.98 }}
                onClick={handleSave}
                className="w-full h-11 rounded-xl font-medium text-[14px] text-white transition-all duration-300 flex items-center justify-center gap-2"
                style={{
                  background: isSaved ? '#16a34a' : '#2094f3',
                  boxShadow: isSaved
                    ? '0 4px 14px -2px rgba(22,163,74,0.3)'
                    : '0 4px 14px -2px rgba(32,148,243,0.3)',
                }}
              >
                <AnimatePresence mode="wait">
                  {isSaved ? (
                    <motion.span
                      key="saved"
                      initial={{ opacity: 0, scale: 0.8 }}
                      animate={{ opacity: 1, scale: 1 }}
                      exit={{ opacity: 0 }}
                      className="flex items-center gap-2"
                    >
                      <Check className="w-4 h-4" /> Saved!
                    </motion.span>
                  ) : (
                    <motion.span key="save" initial={{ opacity: 0 }} animate={{ opacity: 1 }}>
                      Save Configuration
                    </motion.span>
                  )}
                </AnimatePresence>
              </motion.button>
            </div>
          </Card>
        </div>

        {/* Preferences */}
        <div className="space-y-2">
          <SectionHeader title="Preferences" />
          <Card>
            <div className="p-4 space-y-4 border-b border-[#f1f5f9]">
              <div className="flex items-center gap-3 mb-3">
                <div className="w-8 h-8 rounded-full bg-[#f3e8ff] flex items-center justify-center flex-shrink-0">
                  <Moon className="w-4 h-4 text-purple-600" />
                </div>
                <div>
                  <p className="text-[14px] font-medium text-slate-900">Theme Appearance</p>
                  <p className="text-[12px] text-slate-500">Choose how Vira looks</p>
                </div>
              </div>
              
              <div className="grid grid-cols-3 gap-2">
                {[
                  { id: 'light', label: 'Light', icon: Sun },
                  { id: 'dark', label: 'Dark', icon: Moon },
                  { id: 'system', label: 'System', icon: Globe }
                ].map((theme) => (
                  <button
                    key={theme.id}
                    onClick={() => setDarkMode(theme.id === 'dark')}
                    className={`flex flex-col items-center justify-center py-3 rounded-xl border transition-all ${
                      (theme.id === 'dark' && darkMode) || (theme.id === 'light' && !darkMode)
                        ? 'border-blue-500 bg-blue-50/50 text-blue-600 shadow-sm'
                        : 'border-slate-200 bg-white text-slate-500 hover:bg-slate-50'
                    }`}
                  >
                    <theme.icon className={`w-5 h-5 mb-1.5 ${((theme.id === 'dark' && darkMode) || (theme.id === 'light' && !darkMode)) ? 'text-blue-600' : 'text-slate-400'}`} />
                    <span className="text-[11px] font-medium">{theme.label}</span>
                  </button>
                ))}
              </div>
            </div>

            <ToggleRow
              icon={<Volume2 className="w-4 h-4 text-blue-600" />}
              iconBg="#e0f2fe"
              title="Voice Output"
              subtitle="Read responses aloud"
              checked={voiceOutput}
              onChange={setVoiceOutput}
              isFirst
            />
            <ToggleRow
              icon={<Globe className="w-4 h-4 text-emerald-600" />}
              iconBg="#d1fae5"
              title="Use Web Browsing"
              subtitle="Allow Vira to search the internet"
              checked={webBrowsing}
              onChange={setWebBrowsing}
            />
            <ToggleRow
              icon={<Zap className="w-4 h-4 text-orange-600" />}
              iconBg="#ffedd5"
              title="Haptics"
              subtitle="Vibrate on interaction"
              checked={haptics}
              onChange={setHaptics}
            />
            <ToggleRow
              icon={<Bell className="w-4 h-4 text-pink-600" />}
              iconBg="#fce7f3"
              title="Notifications"
              subtitle="Reminders & updates from Vira"
              checked={notifications}
              onChange={setNotifications}
            />
          </Card>
        </div>

        {/* AI Behaviour */}
        <div className="space-y-2">
          <SectionHeader title="AI Behaviour" />
          <Card>
            <ToggleRow
              icon={<BrainCircuit className="w-4 h-4 text-indigo-600" />}
              iconBg="#e0e7ff"
              title="Memory Mode"
              subtitle="Remember context across sessions"
              checked={memoryMode}
              onChange={setMemoryMode}
              isFirst
            />
            <ToggleRow
              icon={<Shield className="w-4 h-4 text-slate-500" />}
              iconBg="#f1f5f9"
              title="Privacy Mode"
              subtitle="Don't save conversation history"
              checked={privacyMode}
              onChange={setPrivacyMode}
            />
            <SelectRow
              icon={<Languages className="w-4 h-4 text-sky-600" />}
              iconBg="#e0f2fe"
              title="Language"
              value={language}
              options={['English', 'Indonesia', '日本語', 'Español', 'Français']}
              onChange={setLanguage}
            />
            <SelectRow
              icon={<MessageSquare className="w-4 h-4 text-teal-600" />}
              iconBg="#ccfbf1"
              title="Response Style"
              value={responseStyle}
              options={['Concise', 'Balanced', 'Detailed', 'Creative']}
              onChange={setResponseStyle}
            />
          </Card>
        </div>

        {/* Danger Zone */}
        <div className="space-y-2">
          <SectionHeader title="Data" />
          <Card>
            <div
              className="px-4 py-3.5"
              style={{ borderBottom: '1px solid #f1f5f9' }}
            >
              <AnimatePresence>
                {showClearConfirm ? (
                  <motion.div
                    initial={{ opacity: 0, height: 0 }}
                    animate={{ opacity: 1, height: 'auto' }}
                    exit={{ opacity: 0, height: 0 }}
                    className="space-y-3"
                  >
                    <p className="text-[13px] text-slate-600">
                      This will delete all your conversation history with Vira. This action cannot be undone.
                    </p>
                    <div className="flex gap-2">
                      <button
                        onClick={() => setShowClearConfirm(false)}
                        className="flex-1 h-9 rounded-xl bg-slate-100 text-slate-600 text-[13px] font-medium hover:bg-slate-200 transition-colors"
                      >
                        Cancel
                      </button>
                      <button
                        onClick={() => {
                          setShowClearConfirm(false);
                          navigate('/');
                        }}
                        className="flex-1 h-9 rounded-xl bg-red-500 text-white text-[13px] font-medium hover:bg-red-600 transition-colors"
                      >
                        Clear All
                      </button>
                    </div>
                  </motion.div>
                ) : (
                  <button
                    onClick={() => setShowClearConfirm(true)}
                    className="w-full flex items-center gap-3 text-left"
                  >
                    <div className="w-8 h-8 rounded-full bg-red-50 flex items-center justify-center flex-shrink-0">
                      <Trash2 className="w-4 h-4 text-red-500" />
                    </div>
                    <div className="flex-1">
                      <p className="text-[14px] font-medium text-slate-900">Clear Chat History</p>
                      <p className="text-[12px] text-slate-500">Remove all conversations</p>
                    </div>
                    <ChevronRight className="w-4 h-4 text-slate-300" />
                  </button>
                )}
              </AnimatePresence>
            </div>
          </Card>
        </div>

        {/* App Features */}
        <div className="space-y-2">
          <SectionHeader title="App Features" />
          <Card>
            <LinkRow
              icon={<Star className="w-5 h-5 text-amber-500" />}
              title="Productivity Analytics"
              subtitle="View your daily insights"
              onClick={() => navigate('/analytics')}
              isFirst
            />
            <LinkRow
              icon={<Zap className="w-5 h-5 text-blue-500" />}
              title="Customize Actions"
              subtitle="Edit quick action buttons"
              onClick={() => navigate('/customize-actions')}
            />
            <LinkRow
              icon={<HelpCircle className="w-5 h-5 text-purple-500" />}
              title="View Onboarding"
              subtitle="Replay the welcome tour"
              onClick={() => navigate('/onboarding')}
            />
          </Card>
        </div>

        {/* Support & Account */}
        <div className="space-y-2">
          <SectionHeader title="Support & Account" />
          <Card>
            <LinkRow
              icon={<HelpCircle className="w-5 h-5 text-slate-400" />}
              title="Help & Support"
              subtitle="FAQs, tutorials, contact us"
              isFirst
            />
            <LinkRow
              icon={<Star className="w-5 h-5 text-yellow-400" />}
              title="Rate Vira"
              subtitle="Share your experience"
            />
            <LinkRow
              icon={<LogOut className="w-5 h-5 text-red-500" />}
              title="Sign Out"
              danger
            />
          </Card>
        </div>

        {/* Footer */}
        <div className="text-center space-y-1 pb-4">
          <div
            className="flex items-center justify-center gap-1.5"
          >
            <div
              className="w-5 h-5 rounded-md flex items-center justify-center"
              style={{ background: 'linear-gradient(135deg, #6366f1, #9333ea)' }}
            >
              <span className="text-white" style={{ fontSize: 10, fontWeight: 800 }}>V</span>
            </div>
            <span className="text-[13px] font-semibold text-slate-500">Vira AI</span>
          </div>
          <p className="text-[12px] text-slate-400">Version 2.4.0 (Build 1024)</p>
          <p className="text-[11px] text-slate-300">Made with ✨ for a smarter life</p>
        </div>

      </main>
    </div>
  );
}
