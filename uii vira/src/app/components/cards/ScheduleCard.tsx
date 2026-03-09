import { Calendar, Clock, MapPin } from 'lucide-react';

export interface ScheduleItem {
  time: string;
  title: string;
  location: string;
  color: string;
  duration?: string;
  participants?: string[];
}

export function ScheduleCard({ items, title }: { items: ScheduleItem[]; title: string }) {
  return (
    <div className="space-y-4 w-full">
      <div className="flex items-center gap-2 mb-1">
        <div className="w-6 h-6 rounded-full bg-white/10 flex items-center justify-center">
          <Calendar className="w-3.5 h-3.5 text-slate-300" />
        </div>
        <p className="text-white font-medium text-[14px]">{title}</p>
      </div>
      
      <div className="relative pl-3 space-y-4">
        {/* Timeline line */}
        <div className="absolute left-[15px] top-2 bottom-2 w-px bg-white/10" />
        
        {items.map((item, i) => (
          <div key={i} className="relative flex gap-4 items-start group cursor-pointer">
            {/* Timeline dot */}
            <div 
              className="absolute -left-[3px] top-1.5 w-2 h-2 rounded-full border-2 border-[#101622] shadow-[0_0_0_2px_rgba(255,255,255,0.05)] z-10 group-hover:scale-150 transition-transform"
              style={{ backgroundColor: item.color }}
            />
            
            <div className="w-16 flex-shrink-0 pt-0.5">
              <span className="text-xs font-semibold text-slate-300 tracking-tight">{item.time.split(' ')[0]}</span>
              <span className="text-[10px] text-slate-500 ml-1">{item.time.split(' ')[1]}</span>
            </div>
            
            <div className="flex-1 bg-white/5 border border-white/5 rounded-2xl p-3 hover:bg-white/10 transition-colors relative overflow-hidden group-hover:border-white/10">
              <div className="absolute top-0 left-0 bottom-0 w-1 opacity-60" style={{ backgroundColor: item.color }} />
              
              <h4 className="text-white font-medium text-[14px] leading-tight mb-1.5">{item.title}</h4>
              
              <div className="flex flex-col gap-1.5">
                <div className="flex items-center gap-1.5 text-slate-400">
                  <MapPin className="w-3 h-3 flex-shrink-0" />
                  <span className="text-xs truncate">{item.location}</span>
                </div>
                
                {(item.duration || item.participants) && (
                  <div className="flex items-center gap-3 mt-0.5">
                    {item.duration && (
                      <div className="flex items-center gap-1 text-slate-500">
                        <Clock className="w-3 h-3" />
                        <span className="text-[10px]">{item.duration}</span>
                      </div>
                    )}
                    
                    {item.participants && (
                      <div className="flex items-center -space-x-1.5">
                        {item.participants.map((p, idx) => (
                          <div key={idx} className="w-4 h-4 rounded-full bg-slate-700 border border-[#1e2a3a] flex items-center justify-center text-[8px] font-bold text-white">
                            {p}
                          </div>
                        ))}
                      </div>
                    )}
                  </div>
                )}
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
