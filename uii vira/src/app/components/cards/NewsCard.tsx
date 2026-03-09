import { ChevronRight, ExternalLink } from 'lucide-react';
import { motion } from 'motion/react';

export interface NewsItem {
  category: string;
  title: string;
  thumbnail?: string;
  source?: string;
  time?: string;
}

export function NewsCard({ items }: { items: NewsItem[] }) {
  return (
    <div className="space-y-3 w-full">
      <div className="flex items-center justify-between mb-1">
        <h4 className="text-white font-semibold text-sm tracking-tight">Today's Top Stories</h4>
        <button className="text-xs text-[#25d1f4] flex items-center hover:text-white transition-colors">
          View all <ChevronRight className="w-3 h-3 ml-0.5" />
        </button>
      </div>
      
      <div className="space-y-3">
        {items.map((item, i) => (
          <motion.div 
            key={i} 
            whileHover={{ x: 2 }}
            className="group flex gap-3 pb-3 border-b border-white/5 last:border-0 last:pb-0 cursor-pointer"
          >
            {item.thumbnail ? (
              <div className="w-16 h-16 rounded-xl overflow-hidden flex-shrink-0 bg-white/5 border border-white/10 relative">
                <img src={item.thumbnail} alt="" className="w-full h-full object-cover opacity-80 group-hover:opacity-100 transition-opacity" />
                <div className="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent opacity-0 group-hover:opacity-100 transition-opacity" />
              </div>
            ) : (
              <div className="w-16 h-16 rounded-xl bg-white/5 border border-white/10 flex items-center justify-center flex-shrink-0 group-hover:bg-white/10 transition-colors">
                <span className="text-2xl">{item.category.split(' ')[0]}</span>
              </div>
            )}
            
            <div className="flex-1 min-w-0 py-0.5 flex flex-col justify-between">
              <div>
                <p className="text-[10px] text-[#a855f7] font-semibold tracking-wider uppercase mb-0.5">
                  {item.category.replace(/[^a-zA-Z]/g, '')}
                </p>
                <p className="text-slate-100 text-[13px] font-medium leading-snug line-clamp-2 group-hover:text-white transition-colors">
                  {item.title}
                </p>
              </div>
              <div className="flex items-center justify-between mt-1.5">
                <span className="text-[10px] text-slate-500">{item.source || 'Global News'} • {item.time || '2h ago'}</span>
                <ExternalLink className="w-3 h-3 text-slate-500 opacity-0 group-hover:opacity-100 transition-opacity" />
              </div>
            </div>
          </motion.div>
        ))}
      </div>
    </div>
  );
}
