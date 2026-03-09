import { motion } from "motion/react";
import { ExternalLink, Sparkles, Share2, Bookmark } from "lucide-react";
import { Card, CardHeader, CardContent, CardFooter } from "./ui/card";
import { Button } from "./ui/button";

export function AIResponseCard() {
  const container = {
    hidden: { opacity: 0 },
    show: {
      opacity: 1,
      transition: {
        staggerChildren: 0.1
      }
    }
  };

  const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 }
  };

  return (
    <motion.div
      variants={container}
      initial="hidden"
      animate="show"
      className="w-full"
    >
      <Card className="overflow-hidden border-purple-100 shadow-xl shadow-purple-900/5 rounded-3xl bg-white/80 backdrop-blur-sm">
        {/* Header */}
        <CardHeader className="border-b border-purple-50 pb-4">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-purple-500 to-purple-600 flex items-center justify-center shadow-lg shadow-purple-500/20">
              <Sparkles className="w-5 h-5 text-white" />
            </div>
            <div className="flex-1">
              <p className="text-gray-900 font-semibold text-sm">AI Response</p>
              <p className="text-purple-400 text-xs font-medium">Just now</p>
            </div>
            <Button variant="ghost" size="icon" className="text-gray-400 hover:text-purple-600 rounded-full h-8 w-8">
              <Bookmark className="w-4 h-4" />
            </Button>
          </div>
        </CardHeader>

        {/* Content */}
        <CardContent className="pt-6 space-y-6">
          {/* Introduction */}
          <motion.p variants={item} className="text-gray-600 leading-relaxed text-[15px]">
            Meditation offers numerous benefits for mental health, supported by extensive research. 
            Here are the key advantages:
          </motion.p>

          {/* Bullet Points */}
          <div className="space-y-4">
            {[
              {
                title: "Reduces Stress and Anxiety",
                description: "Regular meditation practice lowers cortisol levels and activates the parasympathetic nervous system, promoting relaxation."
              },
              {
                title: "Improves Focus and Concentration",
                description: "Meditation strengthens attention span and enhances cognitive function by training the mind to stay present."
              },
              {
                title: "Enhances Emotional Well-being",
                description: "Helps regulate emotions, increases self-awareness, and promotes a more positive outlook on life."
              },
              {
                title: "Better Sleep Quality",
                description: "Meditation techniques can help calm racing thoughts and prepare the body for restful sleep."
              }
            ].map((point, idx) => (
              <motion.div key={idx} variants={item} className="group flex gap-4 p-3 rounded-2xl hover:bg-purple-50/50 transition-colors duration-300">
                <div className="mt-2 w-1.5 h-1.5 rounded-full bg-purple-400 group-hover:bg-purple-600 transition-colors flex-shrink-0" />
                <div>
                  <h3 className="text-gray-900 font-semibold text-sm mb-1 group-hover:text-purple-700 transition-colors">{point.title}</h3>
                  <p className="text-gray-500 text-sm leading-relaxed">
                    {point.description}
                  </p>
                </div>
              </motion.div>
            ))}
          </div>

          {/* Conclusion */}
          <motion.p variants={item} className="text-gray-600 leading-relaxed text-[15px] pt-2 border-t border-dashed border-gray-100">
            These benefits can typically be experienced with just 10-15 minutes of daily practice, 
            making meditation an accessible and effective tool for mental wellness.
          </motion.p>
        </CardContent>

        {/* Source Link */}
        <div className="px-6 pb-2">
          <motion.div variants={item}>
            <Button 
              variant="secondary" 
              className="w-full justify-between bg-purple-50 hover:bg-purple-100 text-purple-700 rounded-xl h-auto py-3 group border border-purple-100"
            >
              <div className="flex items-center gap-2">
                <ExternalLink className="w-4 h-4 text-purple-500 group-hover:text-purple-700" />
                <span className="text-sm font-medium">Source: Research on Meditation</span>
              </div>
              <motion.span 
                initial={{ x: 0 }}
                whileHover={{ x: 3 }}
              >
                <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                </svg>
              </motion.span>
            </Button>
          </motion.div>
        </div>

        {/* Action Buttons */}
        <CardFooter className="gap-3 pt-4">
          <Button variant="outline" className="flex-1 rounded-xl h-11 border-gray-200 text-gray-600 hover:text-purple-700 hover:border-purple-200 hover:bg-purple-50">
            <Bookmark className="w-4 h-4 mr-2" />
            Save
          </Button>
          <Button variant="outline" className="flex-1 rounded-xl h-11 border-gray-200 text-gray-600 hover:text-purple-700 hover:border-purple-200 hover:bg-purple-50">
            <Share2 className="w-4 h-4 mr-2" />
            Share
          </Button>
        </CardFooter>
      </Card>
    </motion.div>
  );
}
