import { Cloud, Sun, Droplets, Wind } from 'lucide-react';

export interface WeatherData {
  city: string;
  temp: string;
  condition: string;
  humidity: string;
  uv: string;
  tomorrow: string;
}

export function WeatherCard({ data }: { data: WeatherData }) {
  return (
    <div className="space-y-3.5 w-full">
      <div className="flex items-center justify-between">
        <div>
          <h3 className="text-white font-bold text-3xl tracking-tight leading-none mb-1">{data.temp}</h3>
          <p className="text-slate-200 font-medium text-sm">{data.condition}</p>
          <p className="text-slate-400 text-xs mt-0.5">{data.city}</p>
        </div>
        <div className="w-16 h-16 rounded-full bg-gradient-to-br from-yellow-400/20 to-orange-500/20 border border-yellow-400/30 flex items-center justify-center shadow-[0_0_15px_rgba(250,204,21,0.15)]">
          <Sun className="w-8 h-8 text-yellow-400 drop-shadow-md" />
        </div>
      </div>
      
      <div className="grid grid-cols-3 gap-2">
        <div className="bg-white/5 border border-white/10 rounded-xl p-2.5 flex flex-col items-center justify-center gap-1.5 hover:bg-white/10 transition-colors">
          <Droplets className="w-4 h-4 text-blue-400" />
          <div className="text-center">
            <p className="text-slate-400 text-[10px] uppercase font-semibold tracking-wider">Humidity</p>
            <p className="text-white text-xs font-medium">{data.humidity}</p>
          </div>
        </div>
        <div className="bg-white/5 border border-white/10 rounded-xl p-2.5 flex flex-col items-center justify-center gap-1.5 hover:bg-white/10 transition-colors">
          <Sun className="w-4 h-4 text-yellow-400" />
          <div className="text-center">
            <p className="text-slate-400 text-[10px] uppercase font-semibold tracking-wider">UV Index</p>
            <p className="text-white text-xs font-medium">{data.uv}</p>
          </div>
        </div>
        <div className="bg-white/5 border border-white/10 rounded-xl p-2.5 flex flex-col items-center justify-center gap-1.5 hover:bg-white/10 transition-colors">
          <Wind className="w-4 h-4 text-teal-400" />
          <div className="text-center">
            <p className="text-slate-400 text-[10px] uppercase font-semibold tracking-wider">Wind</p>
            <p className="text-white text-xs font-medium">12 km/h</p>
          </div>
        </div>
      </div>
      
      <div className="pt-3 border-t border-white/10 flex items-center gap-2">
        <Cloud className="w-4 h-4 text-slate-400" />
        <p className="text-slate-300 text-xs"><span className="text-slate-400">Tomorrow:</span> {data.tomorrow}</p>
      </div>
    </div>
  );
}
