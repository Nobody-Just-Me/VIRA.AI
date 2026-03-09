import { useState, useEffect, useMemo, useCallback } from 'react';
import { motion, AnimatePresence } from 'motion/react';
import { useNavigate } from 'react-router';
import { X, MoreHorizontal, MicOff, ChevronRight } from 'lucide-react';

import { VoiceRecordingIndicator } from '../components/ui/VoiceRecordingIndicator';

type VoiceState = 'idle' | 'listening' | 'processing' | 'done';

const SAMPLE_TRANSCRIPTS = [
  {
    parts: [
      { text: '"What\'s the weather', highlight: '' },
      { text: '"What\'s the weather like in', highlight: '' },
      { text: '"What\'s the weather like in ', highlight: 'Tokyo' },
      { text: '"What\'s the weather like in Tokyo right now?"', highlight: 'Tokyo' },
    ],
  },
  {
    parts: [
      { text: '"Show me my', highlight: '' },
      { text: '"Show me my schedule', highlight: '' },
      { text: '"Show me my schedule for', highlight: '' },
      { text: '"Show me my schedule for today"', highlight: '' },
    ],
  },
  {
    parts: [
      { text: '"Set a reminder', highlight: '' },
      { text: '"Set a reminder for 8 PM', highlight: '' },
      { text: '"Set a reminder for 8 PM tonight"', highlight: '' },
    ],
  },
];

function ViraOrb({ state }: { state: VoiceState }) {
  const isActive = state === 'listening' || state === 'processing';

  return (
    <div className="relative w-64 h-64 flex items-center justify-center">
      {/* Outer glow */}
      <motion.div
        className="absolute inset-0 rounded-full"
        style={{ background: 'rgba(37, 209, 244, 0.25)', filter: 'blur(30px)' }}
        animate={
          isActive
            ? { scale: [1, 1.15, 1], opacity: [0.25, 0.4, 0.25] }
            : { scale: 1, opacity: 0.1 }
        }
        transition={{ duration: 2.5, repeat: Infinity, ease: 'easeInOut' }}
      />

      {/* Outer ring 1 */}
      <motion.div
        className="absolute w-[272px] h-[272px] rounded-full border border-[rgba(37,209,244,0.08)]"
        animate={
          isActive
            ? { scale: [1, 1.08, 1], opacity: [0.3, 0.1, 0.3] }
            : { opacity: 0.1 }
        }
        transition={{ duration: 3, repeat: Infinity, ease: 'easeInOut' }}
      />

      {/* Outer ring 2 */}
      <motion.div
        className="absolute w-[292px] h-[292px] rounded-full border border-[rgba(168,85,247,0.12)]"
        animate={
          isActive
            ? { scale: [1, 1.12, 1], opacity: [0.2, 0.06, 0.2] }
            : { opacity: 0.06 }
        }
        transition={{ duration: 4, repeat: Infinity, ease: 'easeInOut', delay: 0.5 }}
      />

      {/* Main orb container */}
      <motion.div
        className="relative w-48 h-48 rounded-full flex items-center justify-center"
        animate={
          isActive
            ? { scale: [1, 1.04, 1] }
            : { scale: 1 }
        }
        transition={{ duration: 2, repeat: Infinity, ease: 'easeInOut' }}
        style={{
          background: 'linear-gradient(45deg, rgba(37, 209, 244, 0.1) 0%, rgba(168, 85, 247, 0.1) 100%)',
          boxShadow: isActive
            ? '0 0 50px 0 rgba(37, 209, 244, 0.3), inset 0 0 30px rgba(168,85,247,0.1)'
            : '0 0 20px 0 rgba(37, 209, 244, 0.1)',
          border: '1px solid rgba(255, 255, 255, 0.1)',
          backdropFilter: 'blur(2px)',
        }}
      >
        {/* Inner orb */}
        <motion.div
          className="w-32 h-32 rounded-full"
          animate={
            isActive
              ? { rotate: 360, scale: [1, 1.05, 1] }
              : { rotate: 0, scale: 1 }
          }
          transition={{
            rotate: { duration: 20, repeat: Infinity, ease: 'linear' },
            scale: { duration: 2.5, repeat: Infinity, ease: 'easeInOut' },
          }}
          style={{
            background: 'linear-gradient(225deg, rgba(168, 85, 247, 0.25) 0%, rgba(37, 209, 244, 0.25) 100%)',
            border: '1px solid rgba(255, 255, 255, 0.2)',
          }}
        />

        {/* Vira "V" logo */}
        <div className="absolute inset-0 flex items-center justify-center">
          <motion.span
            className="text-white select-none"
            style={{
              fontSize: 40,
              fontWeight: 700,
              letterSpacing: '-2px',
              background: 'linear-gradient(135deg, #25d1f4, #c084fc)',
              WebkitBackgroundClip: 'text',
              WebkitTextFillColor: 'transparent',
            }}
            animate={
              state === 'processing'
                ? { opacity: [1, 0.4, 1] }
                : { opacity: 1 }
            }
            transition={{ duration: 1.2, repeat: Infinity }}
          >
            V
          </motion.span>
        </div>
      </motion.div>
    </div>
  );
}

export default function VoiceActive() {
  const navigate = useNavigate();
  const [voiceState, setVoiceState] = useState<VoiceState>('listening');
  const [transcriptIndex, setTranscriptIndex] = useState(0);
  const [partIndex, setPartIndex] = useState(0);
  const [isCancelling, setIsCancelling] = useState(false);

  const selectedTranscript = useMemo(
    () => SAMPLE_TRANSCRIPTS[transcriptIndex],
    [transcriptIndex]
  );

  const currentPart = selectedTranscript.parts[partIndex] || selectedTranscript.parts[selectedTranscript.parts.length - 1];
  const finalPart = selectedTranscript.parts[selectedTranscript.parts.length - 1];
  const isComplete = partIndex >= selectedTranscript.parts.length - 1;

  // Simulate live transcription
  useEffect(() => {
    if (voiceState !== 'listening') return;

    const interval = setInterval(() => {
      setPartIndex((prev) => {
        if (prev < selectedTranscript.parts.length - 1) {
          return prev + 1;
        } else {
          clearInterval(interval);
          // Transition to processing
          setTimeout(() => {
            setVoiceState('processing');
            setTimeout(() => {
              setVoiceState('done');
            }, 2200);
          }, 600);
          return prev;
        }
      });
    }, 800);

    return () => clearInterval(interval);
  }, [voiceState, selectedTranscript]);

  const handleSend = useCallback(() => {
    const query = finalPart.text.replace(/^"|"$/g, '');
    navigate('/', { state: { voiceQuery: query } });
  }, [navigate, finalPart]);

  const handleCancel = useCallback(() => {
    setIsCancelling(true);
    setTimeout(() => navigate(-1), 400);
  }, [navigate]);

  const handleRetry = useCallback(() => {
    const next = (transcriptIndex + 1) % SAMPLE_TRANSCRIPTS.length;
    setTranscriptIndex(next);
    setPartIndex(0);
    setVoiceState('listening');
  }, [transcriptIndex]);

  const statusLabel = {
    idle: 'READY',
    listening: 'LISTENING',
    processing: 'PROCESSING',
    done: 'DONE',
  }[voiceState];

  const statusColor = {
    idle: '#94a3b8',
    listening: '#25d1f4',
    processing: '#a855f7',
    done: '#22c55e',
  }[voiceState];

  return (
    <motion.div
      initial={{ opacity: 0 }}
      animate={{ opacity: isCancelling ? 0 : 1 }}
      transition={{ duration: 0.3 }}
      className="fixed inset-0 text-white overflow-hidden flex flex-col font-sans"
      style={{ background: '#101f22' }}
    >
      {/* Background atmosphere */}
      <div className="absolute inset-0 z-0 pointer-events-none overflow-hidden">
        <div
          className="absolute inset-0"
          style={{
            background: 'linear-gradient(180deg, #0f172a 0%, #101f22 50%, #0a1416 100%)',
            opacity: 0.95,
          }}
        />
        <motion.div
          animate={{ scale: [1, 1.1, 1], opacity: [0.15, 0.25, 0.15] }}
          transition={{ duration: 8, repeat: Infinity, ease: 'easeInOut' }}
          className="absolute top-0 left-[-16px] w-96 h-96 rounded-full"
          style={{ background: 'rgba(37, 209, 244, 0.2)', filter: 'blur(50px)', mixBlendMode: 'screen' }}
        />
        <motion.div
          animate={{ scale: [1, 1.2, 1], opacity: [0.15, 0.25, 0.15] }}
          transition={{ duration: 10, repeat: Infinity, ease: 'easeInOut', delay: 1 }}
          className="absolute top-0 right-[-16px] w-96 h-96 rounded-full"
          style={{ background: 'rgba(168, 85, 247, 0.2)', filter: 'blur(50px)', mixBlendMode: 'screen' }}
        />
        <motion.div
          animate={{ scale: [1, 1.1, 1], opacity: [0.15, 0.25, 0.15] }}
          transition={{ duration: 9, repeat: Infinity, ease: 'easeInOut', delay: 2 }}
          className="absolute bottom-[-32px] left-20 w-96 h-96 rounded-full"
          style={{ background: 'rgba(37, 99, 235, 0.2)', filter: 'blur(50px)', mixBlendMode: 'screen' }}
        />
      </div>

      {/* Header */}
      <header className="relative z-10 w-full px-6 pt-12 pb-4 flex justify-between items-center">
        <motion.button
          whileTap={{ scale: 0.9 }}
          onClick={handleCancel}
          className="w-10 h-10 rounded-full flex items-center justify-center"
          style={{ background: 'rgba(255,255,255,0.05)', backdropFilter: 'blur(2px)' }}
        >
          <X className="w-5 h-5 text-white/70" />
        </motion.button>

        <div className="flex items-center gap-2.5">
          <motion.div
            className="w-2 h-2 rounded-full"
            style={{ background: statusColor }}
            animate={
              voiceState === 'listening'
                ? { scale: [1, 1.4, 1], opacity: [1, 0.5, 1] }
                : voiceState === 'processing'
                ? { scale: [1, 1.2, 1], opacity: [1, 0.7, 1] }
                : { scale: 1, opacity: 1 }
            }
            transition={{ duration: 1, repeat: voiceState === 'done' ? 0 : Infinity }}
          />
          <span
            className="text-[13px] font-medium tracking-[0.3em] uppercase"
            style={{ color: 'rgba(255,255,255,0.5)' }}
          >
            {statusLabel}
          </span>
        </div>

        <motion.button
          whileTap={{ scale: 0.9 }}
          className="w-10 h-10 rounded-full flex items-center justify-center"
          style={{ background: 'rgba(255,255,255,0.05)', backdropFilter: 'blur(2px)' }}
        >
          <MoreHorizontal className="w-5 h-5 text-white/70" />
        </motion.button>
      </header>

      {/* Center Visualization */}
      <div className="relative z-10 flex-1 flex flex-col items-center justify-center w-full px-6">

        {/* Orb */}
        <ViraOrb state={voiceState} />

        {/* Waveform (hidden when done) */}
        <AnimatePresence>
          {voiceState !== 'done' && (
            <motion.div
              initial={{ opacity: 0, height: 0 }}
              animate={{ opacity: 1, height: 'auto' }}
              exit={{ opacity: 0, height: 0 }}
              className="w-full max-w-xs mt-2 mb-2"
            >
              <VoiceRecordingIndicator isActive={voiceState === 'listening'} />
            </motion.div>
          )}
        </AnimatePresence>

        {/* Transcription */}
        <div className="w-full text-center space-y-3 mt-4 px-4">
          <AnimatePresence mode="popLayout">
            <motion.div
              key={currentPart.text}
              initial={{ opacity: 0, y: 8, filter: 'blur(4px)' }}
              animate={{ opacity: 1, y: 0, filter: 'blur(0px)' }}
              exit={{ opacity: 0, y: -8 }}
              transition={{ duration: 0.3 }}
              className="text-[28px] font-semibold text-white tracking-tight leading-tight"
            >
              {currentPart.highlight ? (
                <>
                  {currentPart.text.split(currentPart.highlight).map((part, i, arr) => (
                    <span key={i}>
                      {part}
                      {i < arr.length - 1 && (
                        <span
                          style={{
                            background: 'linear-gradient(90deg, #25d1f4, #c084fc)',
                            WebkitBackgroundClip: 'text',
                            WebkitTextFillColor: 'transparent',
                          }}
                        >
                          {currentPart.highlight}
                        </span>
                      )}
                    </span>
                  ))}
                </>
              ) : (
                currentPart.text || (
                  <span style={{ color: 'rgba(255,255,255,0.3)', fontWeight: 300, fontSize: 20 }}>
                    Listening...
                  </span>
                )
              )}
            </motion.div>
          </AnimatePresence>

          <AnimatePresence mode="wait">
            <motion.p
              key={voiceState}
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              exit={{ opacity: 0 }}
              className="text-[16px] font-light"
              style={{ color: 'rgba(255,255,255,0.35)' }}
            >
              {voiceState === 'listening' && 'Listening...'}
              {voiceState === 'processing' && 'Processing request...'}
              {voiceState === 'done' && 'Ready to send'}
            </motion.p>
          </AnimatePresence>
        </div>
      </div>

      {/* Footer Controls */}
      <div className="relative z-10 w-full px-6 pb-10 flex flex-col items-center gap-3">

        {/* Send button (visible when done) */}
        <AnimatePresence>
          {voiceState === 'done' && (
            <motion.button
              initial={{ opacity: 0, y: 16, scale: 0.9 }}
              animate={{ opacity: 1, y: 0, scale: 1 }}
              exit={{ opacity: 0, y: 8, scale: 0.9 }}
              whileTap={{ scale: 0.96 }}
              onClick={handleSend}
              className="flex items-center gap-2 px-8 py-3.5 rounded-full font-medium text-white"
              style={{
                background: 'linear-gradient(135deg, #6366f1, #9333ea)',
                boxShadow: '0 8px 24px -4px rgba(99,102,241,0.5)',
              }}
            >
              <span>Send to Vira</span>
              <ChevronRight className="w-4 h-4" />
            </motion.button>
          )}
        </AnimatePresence>

        {/* Cancel / Retry button */}
        <div className="flex gap-3 items-center">
          <motion.button
            whileTap={{ scale: 0.95 }}
            onClick={voiceState === 'done' ? handleRetry : handleCancel}
            className="relative flex items-center gap-3 px-8 py-4 rounded-full"
            style={{
              background: voiceState === 'done'
                ? 'rgba(255,255,255,0.05)'
                : 'rgba(239,68,68,0.1)',
              backdropFilter: 'blur(12px)',
              border: voiceState === 'done'
                ? '1px solid rgba(255,255,255,0.1)'
                : '1px solid rgba(239,68,68,0.2)',
            }}
          >
            <MicOff
              className="w-[18px] h-[18px]"
              style={{ color: voiceState === 'done' ? 'rgba(255,255,255,0.5)' : '#f87171' }}
            />
            <span
              className="font-medium tracking-wide text-[14px]"
              style={{ color: voiceState === 'done' ? 'rgba(255,255,255,0.5)' : '#fef2f2' }}
            >
              {voiceState === 'done' ? 'Try Again' : 'Cancel'}
            </span>
          </motion.button>
        </div>

        {/* Bottom handle */}
        <div className="w-28 h-1 rounded-full mt-1" style={{ background: 'rgba(255,255,255,0.15)' }} />
      </div>
    </motion.div>
  );
}
