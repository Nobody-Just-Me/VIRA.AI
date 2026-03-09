import { useState, useEffect } from 'react';
import { Mic, X, Volume2, Sparkles, MessageSquare, ArrowLeft } from 'lucide-react';
import { motion, AnimatePresence } from "motion/react";
import { Button } from "./ui/button";
import { cn } from "@/app/components/ui/utils";

interface VoiceChatProps {
  onBack: () => void;
}

export function VoiceChat({ onBack }: VoiceChatProps) {
  const [isListening, setIsListening] = useState(false);
  const [isSpeaking, setIsSpeaking] = useState(false);
  const [transcript, setTranscript] = useState('');
  const [response, setResponse] = useState('');

  // Simulate voice interaction
  const handleVoiceToggle = () => {
    if (!isListening) {
      setIsListening(true);
      setTranscript('');
      setResponse('');
      
      // Simulate listening
      setTimeout(() => {
        setTranscript('What are some quick exercises I can do at home?');
        setIsListening(false);
        
        // Simulate AI response
        setTimeout(() => {
          setIsSpeaking(true);
          setResponse('Great question! Here are some effective home exercises: First, bodyweight squats are excellent for lower body strength. Second, push-ups target your chest and arms. Third, planks build core stability. Finally, jumping jacks provide cardio. Aim for 3 sets of 10-15 reps each.');
          
          // Simulate speech ending
          setTimeout(() => {
            setIsSpeaking(false);
          }, 5000);
        }, 800);
      }, 2500);
    } else {
      setIsListening(false);
    }
  };

  return (
    <motion.div 
      initial={{ opacity: 0, y: 50 }}
      animate={{ opacity: 1, y: 0 }}
      exit={{ opacity: 0, y: 50 }}
      className="fixed inset-0 z-50 flex flex-col bg-white"
    >
      {/* Background Ambience */}
      <div className="absolute inset-0 overflow-hidden pointer-events-none">
        <div className="absolute top-0 left-0 w-full h-1/2 bg-gradient-to-b from-purple-50/50 to-transparent" />
        <div className="absolute -top-[20%] -right-[20%] w-[500px] h-[500px] rounded-full bg-purple-100/40 blur-3xl" />
        <div className="absolute top-[20%] -left-[10%] w-[300px] h-[300px] rounded-full bg-blue-50/40 blur-3xl" />
      </div>

      {/* Header */}
      <div className="relative z-10 px-6 py-6 flex items-center justify-between">
        <Button
          variant="ghost"
          size="icon"
          onClick={onBack}
          className="rounded-full hover:bg-gray-100 text-gray-500 hover:text-gray-900 transition-colors"
        >
          <X className="w-6 h-6" />
        </Button>
        <div className="flex flex-col items-center">
          <span className="text-sm font-semibold text-gray-900 tracking-wide">VOICE MODE</span>
          <span className="text-[10px] font-medium text-purple-500 uppercase tracking-widest">Live</span>
        </div>
        <div className="w-10" /> {/* Spacer */}
      </div>

      {/* Main Content Area */}
      <div className="flex-1 relative z-10 flex flex-col">
        
        {/* Dynamic Content Area (Transcript/Response) */}
        <div className="flex-1 overflow-y-auto px-6 py-4 space-y-6 flex flex-col justify-center">
          <AnimatePresence mode="wait">
            {!transcript && !response && (
              <motion.div 
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                exit={{ opacity: 0 }}
                className="text-center space-y-6"
              >
                <div className="inline-flex items-center justify-center w-20 h-20 rounded-3xl bg-purple-50 text-purple-600 mb-4 shadow-inner shadow-purple-100">
                  <Mic className="w-8 h-8" />
                </div>
                <div>
                  <h2 className="text-2xl font-bold text-gray-900 mb-2">How can I help?</h2>
                  <p className="text-gray-500">Tap the microphone to start speaking</p>
                </div>
              </motion.div>
            )}

            {transcript && (
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                className="self-end ml-12"
              >
                <div className="bg-gray-100 rounded-2xl rounded-tr-none px-6 py-4 shadow-sm">
                  <p className="text-gray-800 text-lg leading-relaxed">{transcript}</p>
                </div>
                <p className="text-xs text-gray-400 mt-2 text-right font-medium">You</p>
              </motion.div>
            )}

            {response && (
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 0.2 }}
                className="self-start mr-12"
              >
                <div className="bg-gradient-to-br from-purple-600 to-indigo-600 text-white rounded-2xl rounded-tl-none px-6 py-5 shadow-xl shadow-purple-200">
                  <div className="flex items-center gap-2 mb-3 opacity-80">
                    <Sparkles className="w-4 h-4" />
                    <span className="text-xs font-semibold tracking-wide uppercase">AI Assistant</span>
                  </div>
                  <p className="text-lg leading-relaxed font-light">{response}</p>
                </div>
              </motion.div>
            )}
          </AnimatePresence>
        </div>

        {/* Visualizer & Controls */}
        <div className="pb-12 px-6">
          <div className="h-24 mb-8 flex items-center justify-center">
            <WaveformVisualizer isActive={isListening} isSpeaking={isSpeaking} />
          </div>

          <div className="flex items-center justify-center gap-8">
            <Button
              variant="ghost"
              size="icon"
              className="w-12 h-12 rounded-full text-gray-400 hover:text-gray-900 hover:bg-gray-100"
            >
              <MessageSquare className="w-5 h-5" />
            </Button>

            <motion.button
              whileTap={{ scale: 0.95 }}
              onClick={handleVoiceToggle}
              className={cn(
                "w-20 h-20 rounded-full flex items-center justify-center shadow-2xl transition-all duration-500 relative z-20",
                isListening 
                  ? "bg-red-500 shadow-red-200" 
                  : "bg-gray-900 shadow-purple-200 hover:shadow-purple-300"
              )}
            >
              <div className={cn(
                "absolute inset-0 rounded-full border-2 transition-all duration-500",
                isListening ? "border-red-300 scale-125 opacity-100" : "border-gray-200 scale-100 opacity-0"
              )} />
              <div className={cn(
                "absolute inset-0 rounded-full border transition-all duration-500",
                isListening ? "border-red-200 scale-150 opacity-50" : "border-gray-100 scale-100 opacity-0"
              )} />
              
              {isListening ? (
                <div className="w-6 h-6 bg-white rounded-sm animate-pulse" />
              ) : (
                <Mic className="w-8 h-8 text-white" />
              )}
            </motion.button>

            <Button
              variant="ghost"
              size="icon"
              className="w-12 h-12 rounded-full text-gray-400 hover:text-gray-900 hover:bg-gray-100"
            >
              <Volume2 className={cn("w-5 h-5", isSpeaking && "text-purple-600 animate-pulse")} />
            </Button>
          </div>
          
          <div className="mt-8 text-center">
             <p className="text-sm font-medium text-gray-400">
               {isListening ? "Listening..." : isSpeaking ? "Speaking..." : "Tap to speak"}
             </p>
          </div>
        </div>
      </div>
    </motion.div>
  );
}

function WaveformVisualizer({ isActive, isSpeaking }: { isActive: boolean; isSpeaking: boolean }) {
  const bars = 40; // Increased resolution
  
  return (
    <div className="flex items-center justify-center gap-[3px] h-full w-full max-w-xs mx-auto">
      {Array.from({ length: bars }).map((_, i) => (
        <WaveBar 
          key={i} 
          index={i} 
          isActive={isActive} 
          isSpeaking={isSpeaking}
          total={bars}
        />
      ))}
    </div>
  );
}

function WaveBar({ index, isActive, isSpeaking, total }: { index: number; isActive: boolean; isSpeaking: boolean; total: number }) {
  // Calculate center boost
  const center = total / 2;
  const dist = Math.abs(index - center);
  const maxDist = total / 2;
  const bellCurve = 1 - Math.pow(dist / maxDist, 2); // Quadratic falloff

  const [height, setHeight] = useState(4);
  
  // Random fluctuation
  useEffect(() => {
    if (!isActive && !isSpeaking) {
      setHeight(4 * bellCurve + 2);
      return;
    }

    const animate = () => {
      const baseHeight = isSpeaking ? 40 : 25; // Taller for AI speaking
      const noise = Math.random() * baseHeight * 0.5;
      const signal = baseHeight * 0.5 + noise;
      
      // Apply bell curve so center bars are taller
      const value = signal * bellCurve;
      setHeight(Math.max(4, value));
    };

    const interval = setInterval(animate, 50); // Fast update
    return () => clearInterval(interval);
  }, [isActive, isSpeaking, bellCurve]);

  return (
    <motion.div
      layout
      className={cn(
        "w-1 rounded-full transition-colors duration-300",
        isSpeaking ? "bg-purple-500" : isActive ? "bg-red-500" : "bg-gray-200"
      )}
      animate={{ height }}
      transition={{ type: "spring", stiffness: 300, damping: 20 }}
    />
  );
}
