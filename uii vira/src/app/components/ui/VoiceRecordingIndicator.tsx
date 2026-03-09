import { useMemo } from 'react';
import { motion } from 'motion/react';

interface VoiceRecordingIndicatorProps {
  isActive: boolean;
  barCount?: number;
  height?: number;
  minHeight?: number;
  colorFrom?: string;
  colorTo?: string;
}

export function VoiceRecordingIndicator({
  isActive,
  barCount = 28,
  height = 36,
  minHeight = 4,
  colorFrom = '#25d1f4',
  colorTo = '#a855f7',
}: VoiceRecordingIndicatorProps) {
  const waveHeights = useMemo(
    () => Array.from({ length: barCount }, () => Math.floor(Math.random() * height + minHeight)),
    [barCount, height, minHeight]
  );
  
  const waveDelays = useMemo(
    () => Array.from({ length: barCount }, () => Math.random() * 0.6),
    [barCount]
  );
  
  const waveDurations = useMemo(
    () => Array.from({ length: barCount }, () => 0.5 + Math.random() * 0.6),
    [barCount]
  );

  return (
    <div className="flex items-center justify-center gap-0.5 w-full" style={{ height: height + minHeight }}>
      {waveHeights.map((h, i) => (
        <motion.div
          key={i}
          className="rounded-full flex-shrink-0"
          style={{
            width: 3,
            background: `linear-gradient(to top, ${colorFrom}, ${colorTo})`,
            minHeight: minHeight,
          }}
          animate={
            isActive
              ? { height: [minHeight, h, minHeight], opacity: [0.5, 1, 0.5] }
              : { height: minHeight, opacity: 0.2 }
          }
          transition={
            isActive
              ? {
                  duration: waveDurations[i],
                  repeat: Infinity,
                  delay: waveDelays[i],
                  ease: 'easeInOut',
                }
              : {}
          }
        />
      ))}
    </div>
  );
}
