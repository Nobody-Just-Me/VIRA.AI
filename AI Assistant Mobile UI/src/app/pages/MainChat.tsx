import { useState, useRef, useEffect } from 'react';
import { motion, AnimatePresence } from 'motion/react';
import { useNavigate, useLocation } from 'react-router';
import {
  Mic,
  Send,
  Settings as SettingsIcon,
  Sun,
  Newspaper,
  Bell,
  Car,
  AlignLeft,
  Plus,
  Trash2,
  Coffee,
  Cloud,
  Music,
  Sparkles,
} from 'lucide-react';

import { LoadingIndicator } from '../components/ui/LoadingIndicator';
import { WeatherCard } from '../components/cards/WeatherCard';
import { NewsCard } from '../components/cards/NewsCard';
import { ScheduleCard } from '../components/cards/ScheduleCard';

import { PermissionRationaleDialog } from '../components/ui/PermissionRationaleDialog';
import { ContactDisambiguationDialog, Contact } from '../components/ui/ContactDisambiguationDialog';
import { ErrorSnackbar } from '../components/ui/ErrorSnackbar';
import { ChatHistorySidebar } from '../components/ui/ChatHistorySidebar';

// ─── Types ───────────────────────────────────────────────────────────────────

type MessageRole = 'user' | 'ai';
type MessageType = 'text' | 'schedule' | 'weather' | 'news' | 'reminder' | 'traffic';

interface ScheduleItem {
  time: string;
  title: string;
  location: string;
  color: string;
}

interface Message {
  id: number;
  role: MessageRole;
  text: string;
  type: MessageType;
  timestamp: Date;
  schedule?: ScheduleItem[];
  weatherData?: {
    city: string;
    temp: string;
    condition: string;
    humidity: string;
    uv: string;
    tomorrow: string;
  };
  newsItems?: { category: string; title: string }[];
  trafficData?: { route: string; eta: string; status: string; color: string }[];
}

// ─── Smart AI Response Engine ─────────────────────────────────────────────────

function getGreeting(): string {
  const hour = new Date().getHours();
  if (hour < 12) return 'Good Morning';
  if (hour < 17) return 'Good Afternoon';
  return 'Good Evening';
}

function getViraResponse(userInput: string): Omit<Message, 'id' | 'timestamp'> {
  const input = userInput.toLowerCase();

  if (input.match(/weather|cuaca|suhu|temperature|forecast/)) {
    return {
      role: 'ai',
      type: 'weather',
      text: 'weather',
      weatherData: {
        city: 'Jakarta',
        temp: '28°C',
        condition: 'Partly Cloudy ⛅',
        humidity: '74%',
        uv: '7 (High)',
        tomorrow: 'Rain expected in the afternoon 🌧',
      },
    };
  }

  if (input.match(/schedule|jadwal|calendar|meeting|appointment|agenda/)) {
    return {
      role: 'ai',
      type: 'schedule',
      text: "Here's your schedule for today:",
      schedule: [
        { time: '10:00 AM', title: 'Product Review', location: 'Zoom Meeting Room B', color: '#8B5CF6' },
        { time: '12:30 PM', title: 'Lunch with Sarah', location: 'Downtown Bistro', color: '#EAB308' },
        { time: '03:00 PM', title: 'Design Sync', location: 'Office Floor 3', color: '#A855F7' },
        { time: '05:30 PM', title: 'Gym Session', location: 'FitLife Gym', color: '#22C55E' },
      ],
    };
  }

  if (input.match(/news|berita|headline|current event/)) {
    return {
      role: 'ai',
      type: 'news',
      text: "Here are today's top headlines:",
      newsItems: [
        { category: '🤖 Tech', title: 'OpenAI launches new model with advanced multimodal reasoning' },
        { category: '📈 Business', title: 'Global markets rebound after volatile trading week' },
        { category: '🚀 Science', title: 'NASA confirms water ice discovery near lunar south pole' },
        { category: '🌍 World', title: 'G20 summit reaches landmark climate agreement' },
      ],
    };
  }

  if (input.match(/traffic|lalu lintas|macet|route|jalan/)) {
    return {
      role: 'ai',
      type: 'traffic',
      text: "Here's your traffic overview:",
      trafficData: [
        { route: 'Home → Office', eta: '32 min', status: 'Moderate traffic', color: '#EAB308' },
        { route: 'Toll via JORR', eta: '25 min', status: 'Recommended', color: '#22C55E' },
        { route: 'Inner City Route', eta: '48 min', status: 'Heavy traffic', color: '#EF4444' },
      ],
    };
  }

  if (input.match(/reminder|remind|ingat|alarm|notification/)) {
    return {
      role: 'ai',
      type: 'reminder',
      text: "I've set a reminder for you. Here are your active reminders:",
      schedule: [
        { time: '08:00 AM', title: 'Take vitamins 💊', location: 'Daily', color: '#22C55E' },
        { time: '09:30 AM', title: 'Team standup ☕', location: 'Every weekday', color: '#256AF4' },
        { time: '08:00 PM', title: 'Evening walk 🚶', location: 'Daily', color: '#A855F7' },
      ],
    };
  }

  if (input.match(/coffee|kopi|order|pesan/)) {
    return {
      role: 'ai',
      type: 'text',
      text: "☕ I've placed your usual order! \n\n**Flat White, medium, oat milk** from Kopi Kenangan has been ordered. Estimated pickup: 15 minutes. Total: Rp 42.000. The receipt will be sent to your email.",
    };
  }

  if (input.match(/music|lagu|playlist|spotify|play/)) {
    return {
      role: 'ai',
      type: 'text',
      text: "🎵 Playing your **Focus & Flow** playlist on Spotify!\n\nCurrently: *Neon Genesis* by Tycho\nNext up: *Cascade* by Tourist\n\nYou can ask me to skip, pause, or change mood anytime.",
    };
  }

  if (input.match(/hello|hi|halo|hey|selamat/)) {
    return {
      role: 'ai',
      type: 'text',
      text: `Hi there! 👋 I'm Vira, your personal AI assistant. I can help you with your schedule, weather, news, traffic, reminders, and much more. What can I do for you today?`,
    };
  }

  if (input.match(/thank|thanks|terima kasih|makasih/)) {
    return {
      role: 'ai',
      type: 'text',
      text: "You're welcome! 😊 Always here when you need me. Is there anything else I can help you with?",
    };
  }

  const fallbacks = [
    "I understand! Let me process that for you right away. Is there anything specific you'd like me to focus on?",
    "That's an interesting request. I'm analyzing the best approach to help you with this.",
    "Got it! I'll take care of that. Would you like me to also check for related information?",
    "Of course! I'm on it. You can also ask me about your schedule, weather, or latest news while I work on this.",
  ];

  return {
    role: 'ai',
    type: 'text',
    text: fallbacks[Math.floor(Math.random() * fallbacks.length)],
  };
}

// ─── Components ───────────────────────────────────────────────────────────────

function TrafficCard({ data, title }: { data: NonNullable<Message['trafficData']>; title: string }) {
  return (
    <div className="space-y-3">
      <p className="text-slate-100 text-[15px]">{title}</p>
      {data.map((route, i) => (
        <div key={i} className="flex items-center justify-between bg-white/5 rounded-xl px-3 py-2">
          <div>
            <p className="text-white text-[13px] font-medium">{route.route}</p>
            <p className="text-xs" style={{ color: route.color }}>{route.status}</p>
          </div>
          <div className="text-right">
            <p className="text-white font-semibold text-sm">{route.eta}</p>
          </div>
        </div>
      ))}
    </div>
  );
}

function MessageBubble({ msg }: { msg: Message }) {
  const isUser = msg.role === 'user';
  const timeStr = msg.timestamp.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

  const renderContent = () => {
    if (msg.type === 'weather' && msg.weatherData) {
      return <WeatherCard data={msg.weatherData} />;
    }
    if ((msg.type === 'schedule' || msg.type === 'reminder') && msg.schedule) {
      return <ScheduleCard items={msg.schedule} title={msg.text} />;
    }
    if (msg.type === 'news' && msg.newsItems) {
      return <NewsCard items={msg.newsItems} />;
    }
    if (msg.type === 'traffic' && msg.trafficData) {
      return <TrafficCard data={msg.trafficData} title={msg.text} />;
    }
    // Plain text with basic markdown support
    return (
      <p className="text-[15px] leading-relaxed whitespace-pre-line">
        {msg.text.split(/\*\*(.*?)\*\*/g).map((part, i) =>
          i % 2 === 1 ? <strong key={i} className="font-semibold text-white">{part}</strong> : part
        )}
      </p>
    );
  };

  return (
    <motion.div
      initial={{ opacity: 0, y: 12, scale: 0.96 }}
      animate={{ opacity: 1, y: 0, scale: 1 }}
      transition={{ duration: 0.28, ease: [0.34, 1.56, 0.64, 1] }}
      className={`flex w-full ${isUser ? 'justify-end' : 'justify-start'}`}
    >
      <div className={`max-w-[85%] flex flex-col ${isUser ? 'items-end' : 'items-start'} gap-1`}>
        <div className="flex items-center gap-1.5">
          {!isUser && <span className="text-[11px] text-slate-400 ml-1">Vira</span>}
          {isUser && <span className="text-[11px] text-slate-400 mr-1">You</span>}
        </div>

        <div
          className={`px-4 py-3.5 shadow-sm ${
            isUser
              ? 'bg-[#8b5cf6] text-white rounded-t-2xl rounded-bl-2xl rounded-br-[2px] shadow-[0_8px_24px_-4px_rgba(139,92,246,0.35)]'
              : 'bg-white/5 backdrop-blur-md border border-white/[0.08] text-slate-100 rounded-t-2xl rounded-br-2xl rounded-bl-[2px]'
          }`}
        >
          {renderContent()}
        </div>

        <span className="text-[10px] text-slate-600">{timeStr}</span>
      </div>
    </motion.div>
  );
}

// ─── Main Component ───────────────────────────────────────────────────────────

const QUICK_ACTIONS = [
  { icon: Sun, label: 'Weather', color: 'text-yellow-400', query: "What's the weather today?" },
  { icon: Newspaper, label: 'News', color: 'text-blue-400', query: 'Show me today\'s news' },
  { icon: Bell, label: 'Reminders', color: 'text-red-400', query: 'Show my reminders' },
  { icon: Car, label: 'Traffic', color: 'text-green-400', query: 'Check traffic conditions' },
  { icon: Coffee, label: 'Coffee', color: 'text-amber-500', query: 'Order my usual coffee' },
  { icon: Music, label: 'Music', color: 'text-purple-400', query: 'Play some focus music' },
];

const INITIAL_MESSAGES: Message[] = [
  {
    id: 1,
    role: 'ai',
    text: `${getGreeting()}! ✨ I'm **Vira**, your personal AI assistant. I noticed you have a packed schedule today. Would you like a quick briefing to start your day?`,
    type: 'text',
    timestamp: new Date(Date.now() - 60000),
  },
  {
    id: 2,
    role: 'user',
    text: "Yes please, what's on my schedule?",
    type: 'text',
    timestamp: new Date(Date.now() - 50000),
  },
  {
    id: 3,
    role: 'ai',
    text: "Here's your schedule for today:",
    type: 'schedule',
    timestamp: new Date(Date.now() - 45000),
    schedule: [
      { time: '10:00 AM', title: 'Product Review', location: 'Zoom Meeting Room B', color: '#8B5CF6' },
      { time: '12:30 PM', title: 'Lunch with Sarah', location: 'Downtown Bistro', color: '#EAB308' },
    ],
  },
];

export default function MainChat() {
  const navigate = useNavigate();
  const location = useLocation();
  const [input, setInput] = useState('');
  const [messages, setMessages] = useState<Message[]>(INITIAL_MESSAGES);
  const [isTyping, setIsTyping] = useState(false);
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);
  const [showMicPermission, setShowMicPermission] = useState(false);
  const [hasMicPermission, setHasMicPermission] = useState(false);
  
  const [showContactDialog, setShowContactDialog] = useState(false);
  const [contactSearchName, setContactSearchName] = useState('');
  
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const inputRef = useRef<HTMLInputElement>(null);

  // Handle voice transcript coming from VoiceActive
  useEffect(() => {
    const voiceQuery = (location.state as { voiceQuery?: string })?.voiceQuery;
    if (voiceQuery) {
      sendMessage(voiceQuery);
      // Clear state so it doesn't re-trigger
      window.history.replaceState({}, '');
    }
  }, []);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages, isTyping]);

  const sendMessage = (text: string) => {
    if (!text.trim()) return;

    const userMsg: Message = {
      id: Date.now(),
      role: 'user',
      text: text.trim(),
      type: 'text',
      timestamp: new Date(),
    };

    setMessages((prev) => [...prev, userMsg]);
    setInput('');
    setIsTyping(true);

    const typingDelay = 900 + Math.random() * 600;
    
    // Check for special commands
    if (text.toLowerCase().startsWith('call john') || text.toLowerCase().startsWith('message john')) {
      setTimeout(() => {
        setIsTyping(false);
        setContactSearchName('John');
        setShowContactDialog(true);
      }, typingDelay);
      return;
    }

    if (text.toLowerCase().includes('error')) {
      setTimeout(() => {
        setIsTyping(false);
        ErrorSnackbar.show({
          title: "Connection Failed",
          description: "Vira couldn't connect to the server. Please check your internet connection and try again."
        });
      }, typingDelay);
      return;
    }

    setTimeout(() => {
      const response = getViraResponse(text);
      const aiMsg: Message = {
        id: Date.now() + 1,
        ...response,
        timestamp: new Date(),
      };
      setIsTyping(false);
      setMessages((prev) => [...prev, aiMsg]);
    }, typingDelay);
  };

  const handleSend = () => sendMessage(input);

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  const handleQuickAction = (query: string) => {
    sendMessage(query);
    inputRef.current?.focus();
  };

  const handleVoiceClick = () => {
    if (hasMicPermission) {
      navigate('/voice');
    } else {
      setShowMicPermission(true);
    }
  };

  const handleGrantPermission = () => {
    setShowMicPermission(false);
    setHasMicPermission(true);
    navigate('/voice');
  };

  const clearChat = () => {
    setMessages(INITIAL_MESSAGES);
    setIsSidebarOpen(false);
    ErrorSnackbar.success({
      title: "History Cleared",
      description: "All conversation history has been removed."
    });
  };

  const startNewChat = () => {
    setMessages(INITIAL_MESSAGES);
    setIsSidebarOpen(false);
    inputRef.current?.focus();
  };

  const loadPastChat = (id: string, title: string) => {
    // Mock loading a past chat
    setMessages([
      {
        id: Date.now() - 100000,
        role: 'user',
        text: `Loaded previous conversation: ${title}`,
        type: 'text',
        timestamp: new Date(Date.now() - 100000),
      },
      {
        id: Date.now() - 90000,
        role: 'ai',
        text: "Here is the past conversation context. How can we continue from here?",
        type: 'text',
        timestamp: new Date(Date.now() - 90000),
      }
    ]);
    setIsSidebarOpen(false);
  };

  const greeting = getGreeting();

  return (
    <div className="h-screen max-h-screen bg-[#101622] text-slate-200 font-sans flex flex-col relative overflow-hidden">

      {/* Ambient Background */}
      <div className="absolute inset-0 overflow-hidden pointer-events-none z-0">
        <div className="absolute top-[-88px] left-[-39px] w-[195px] h-[442px] bg-[#8b5cf6]/20 blur-[50px] rounded-full" />
        <div className="absolute bottom-[-88px] right-[-39px] w-[234px] h-[530px] bg-[#a855f7]/10 blur-[60px] rounded-full" />
        <div className="absolute top-[353px] left-[117px] w-[117px] h-[265px] bg-[#c084fc]/10 blur-[40px] rounded-full" />
      </div>

      {/* Header */}
      <header className="sticky top-0 z-20 w-full px-5 py-4 flex items-center justify-between bg-[#101622]/60 backdrop-blur-md border-b border-white/5">
        <div className="relative">
          <motion.button
            whileTap={{ scale: 0.92 }}
            onClick={() => setIsSidebarOpen(true)}
            className="w-9 h-9 flex items-center justify-center rounded-xl text-slate-400 hover:text-white hover:bg-white/5 transition-colors"
          >
            <AlignLeft className="w-5 h-5" />
          </motion.button>
        </div>

        <div className="flex flex-col items-center gap-0.5">
          <h1 className="text-[17px] font-semibold text-white/90 tracking-tight leading-none">
            {greeting}
          </h1>
          <div className="flex items-center gap-1.5">
            <div className="w-1.5 h-1.5 rounded-full bg-[#6366f1]" />
            <span className="text-[11px] text-slate-500 tracking-wide">Vira AI</span>
          </div>
        </div>

        <motion.button
          whileTap={{ scale: 0.92 }}
          onClick={() => navigate('/settings')}
          className="w-9 h-9 flex items-center justify-center rounded-xl text-slate-400 hover:text-white hover:bg-white/5 transition-colors"
        >
          <SettingsIcon className="w-5 h-5" />
        </motion.button>
      </header>

      {/* Chat Area */}
      <main className="flex-1 overflow-y-auto px-4 pt-5 pb-52 z-10 space-y-4 scroll-smooth">
        <AnimatePresence initial={false}>
          {messages.map((msg) => (
            <MessageBubble key={msg.id} msg={msg} />
          ))}
          {isTyping && <LoadingIndicator key="typing" />}
        </AnimatePresence>
        <div ref={messagesEndRef} />
      </main>

      {/* Floating Input Area */}
      <div className="absolute bottom-0 left-0 right-0 z-20 bg-gradient-to-t from-[#101622] via-[#101622]/95 to-transparent pt-10 pb-5 px-4">

        {/* Quick Actions */}
        <div
          className="w-full overflow-x-auto pb-3 mb-2"
          style={{ scrollbarWidth: 'none' }}
        >
          <div className="flex gap-2 px-0.5">
            {QUICK_ACTIONS.map((action, idx) => (
              <motion.button
                key={idx}
                whileTap={{ scale: 0.93 }}
                onClick={() => handleQuickAction(action.query)}
                className="flex items-center gap-1.5 bg-white/5 backdrop-blur-md border border-white/10 px-3.5 py-1.5 rounded-full min-w-max hover:bg-white/10 active:bg-white/15 transition-colors"
              >
                <action.icon className={`w-3.5 h-3.5 ${action.color}`} />
                <span className="text-[13px] font-medium text-slate-300">{action.label}</span>
              </motion.button>
            ))}
          </div>
        </div>

        {/* Input Container */}
        <div className="relative w-full max-w-xl mx-auto">
          <div className="backdrop-blur-md bg-[#101622]/60 rounded-[32px] border border-white/[0.08] shadow-[0_0_0_1px_rgba(255,255,255,0.06),0_20px_40px_-8px_rgba(0,0,0,0.5)] flex items-center gap-2 px-1.5 py-1.5">

            {/* New Chat Button */}
            <motion.button
              whileTap={{ scale: 0.92 }}
              onClick={startNewChat}
              className="w-9 h-9 rounded-full bg-white/5 flex-shrink-0 flex items-center justify-center text-slate-400 hover:text-white hover:bg-white/10 transition-colors"
            >
              <Plus className="w-4 h-4" />
            </motion.button>

            <input
              ref={inputRef}
              value={input}
              onChange={(e) => setInput(e.target.value)}
              onKeyDown={handleKeyDown}
              placeholder="Ask Vira anything..."
              className="flex-1 h-10 bg-transparent border-none text-slate-200 placeholder:text-slate-500 focus:outline-none text-[15px] px-1"
            />

            <div className="flex items-center gap-1.5 flex-shrink-0">
              {/* Voice Button */}
              <motion.button
                whileTap={{ scale: 0.9 }}
                onClick={handleVoiceClick}
                className="w-10 h-10 rounded-full bg-white/5 flex items-center justify-center text-white/80 hover:bg-white/10 transition-colors relative overflow-hidden"
              >
                <Mic className="w-[18px] h-[18px] relative z-10" />
              </motion.button>

              {/* Send Button */}
              <motion.button
                whileTap={{ scale: 0.9 }}
                onClick={handleSend}
                disabled={!input.trim()}
                className="w-10 h-10 rounded-full bg-[#8b5cf6] flex items-center justify-center text-white shadow-[0_4px_14px_-2px_rgba(139,92,246,0.5)] hover:bg-[#7c3aed] disabled:opacity-40 disabled:cursor-not-allowed transition-all"
              >
                <Send className="w-4 h-4 ml-0.5" />
              </motion.button>
            </div>
          </div>
        </div>

        {/* Bottom Handle */}
        <div className="w-full flex justify-center mt-3">
          <div className="w-28 h-1 bg-white/15 rounded-full" />
        </div>
      </div>

      {/* Sparkle decoration */}
      <div className="absolute top-20 right-4 opacity-20 pointer-events-none z-0">
        <Sparkles className="w-3 h-3 text-purple-400" />
      </div>

      <PermissionRationaleDialog 
        isOpen={showMicPermission}
        onClose={() => setShowMicPermission(false)}
        onGrant={handleGrantPermission}
        permissionType="microphone"
      />

      <ChatHistorySidebar
        isOpen={isSidebarOpen}
        onClose={() => setIsSidebarOpen(false)}
        onSelectChat={loadPastChat}
        onNewChat={startNewChat}
        onClearHistory={clearChat}
      />

      <ContactDisambiguationDialog
        isOpen={showContactDialog}
        onClose={() => setShowContactDialog(false)}
        searchName={contactSearchName}
        contacts={[
          { id: '1', name: 'John Doe', phone: '+1 (555) 123-4567', label: 'Mobile' },
          { id: '2', name: 'John Smith', phone: '+1 (555) 987-6543', label: 'Work' },
          { id: '3', name: 'Johnny Appleseed', phone: '+1 (555) 555-5555', label: 'Home' },
        ]}
        onSelect={(contact) => {
          setShowContactDialog(false);
          const aiMsg: Message = {
            id: Date.now() + 1,
            role: 'ai',
            type: 'text',
            text: `Calling **${contact.name}** on their ${contact.label.toLowerCase()} number...`,
            timestamp: new Date(),
          };
          setMessages((prev) => [...prev, aiMsg]);
        }}
      />
    </div>
  );
}