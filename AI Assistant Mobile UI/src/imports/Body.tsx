import svgPaths from "./svg-tjx2quwojw";

function Container1() {
  return (
    <div className="content-stretch flex flex-col items-start overflow-clip relative shrink-0 w-full" data-name="Container">
      <div className="flex flex-col font-['Inter:Regular',sans-serif] font-normal justify-center leading-[0] not-italic relative shrink-0 text-[#94a3b8] text-[16px] w-full">
        <p className="leading-[normal] whitespace-pre-wrap">Ask anything...</p>
      </div>
    </div>
  );
}

function Input() {
  return (
    <div className="-translate-y-1/2 absolute content-stretch flex flex-col h-[48px] items-start left-[21px] overflow-clip px-[12px] py-[14px] right-[79px] top-1/2" data-name="Input">
      <Container1 />
    </div>
  );
}

function Container3() {
  return (
    <div className="h-[19px] relative shrink-0 w-[14px]" data-name="Container">
      <svg className="absolute block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 14 19">
        <g id="Container">
          <path d={svgPaths.p39e29d00} fill="var(--fill-0, white)" id="Icon" />
        </g>
      </svg>
    </div>
  );
}

function VoiceInputButtonProminent() {
  return (
    <div className="bg-[rgba(255,255,255,0.05)] content-stretch flex items-center justify-center overflow-clip relative rounded-[9999px] shrink-0 size-[40px]" data-name="Voice Input Button (Prominent)">
      <Container3 />
      <div className="absolute bg-[rgba(37,106,244,0.2)] blur-[6px] inset-0 opacity-0" data-name="Overlay+Blur" />
    </div>
  );
}

function Container4() {
  return (
    <div className="relative shrink-0 size-[13.333px]" data-name="Container">
      <svg className="absolute block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 13.3333 13.3333">
        <g id="Container">
          <path d={svgPaths.p2390e200} fill="var(--fill-0, white)" id="Icon" />
        </g>
      </svg>
    </div>
  );
}

function SendButton() {
  return (
    <div className="bg-[#256af4] content-stretch flex items-center justify-center relative rounded-[9999px] shrink-0 size-[40px]" data-name="Send Button">
      <div className="-translate-y-1/2 absolute bg-[rgba(255,255,255,0)] left-0 rounded-[9999px] shadow-[0px_10px_15px_-3px_rgba(37,106,244,0.3),0px_4px_6px_-4px_rgba(37,106,244,0.3)] size-[40px] top-1/2" data-name="Send Button:shadow" />
      <Container4 />
    </div>
  );
}

function Container2() {
  return (
    <div className="-translate-y-1/2 absolute content-stretch flex gap-[4px] items-center left-[287px] pr-[6px] top-1/2" data-name="Container">
      <VoiceInputButtonProminent />
      <SendButton />
    </div>
  );
}

function OverlayBorderOverlayBlur() {
  return (
    <div className="backdrop-blur-[6px] bg-[rgba(16,22,34,0.6)] h-[62px] relative rounded-[32px] shrink-0 w-full" data-name="Overlay+Border+OverlayBlur">
      <div aria-hidden="true" className="absolute border border-[rgba(255,255,255,0.05)] border-solid inset-0 pointer-events-none rounded-[32px]" />
      <div className="absolute bg-[rgba(255,255,255,0)] inset-0 rounded-[32px] shadow-[0px_0px_0px_1px_rgba(255,255,255,0.1),0px_25px_50px_-12px_rgba(0,0,0,0.25)]" data-name="Overlay+Shadow" />
      <Input />
      <Container2 />
    </div>
  );
}

function Container5() {
  return (
    <div className="content-stretch flex flex-col items-center relative shrink-0 w-full" data-name="Container">
      <div className="bg-[rgba(255,255,255,0.2)] h-[4px] rounded-[9999px] shrink-0 w-[128px]" data-name="Overlay" />
    </div>
  );
}

function Container() {
  return (
    <div className="content-stretch flex flex-col gap-[12px] items-start max-w-[672px] relative shrink-0 w-full" data-name="Container">
      <OverlayBorderOverlayBlur />
      <Container5 />
    </div>
  );
}

function FloatingInputArea() {
  return (
    <div className="absolute bg-gradient-to-t bottom-0 content-stretch flex flex-col from-[#101622] items-start left-0 pb-[20px] pt-[48px] px-[16px] right-0 to-[rgba(16,22,34,0)] via-1/2 via-[rgba(16,22,34,0.9)] z-[4]" data-name="Floating Input Area">
      <Container />
    </div>
  );
}

function Container6() {
  return (
    <div className="h-[12px] relative shrink-0 w-[18px]" data-name="Container">
      <svg className="absolute block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 18 12">
        <g id="Container">
          <path d={svgPaths.p2bce57c0} fill="var(--fill-0, #94A3B8)" id="Icon" />
        </g>
      </svg>
    </div>
  );
}

function Button() {
  return (
    <div className="relative shrink-0" data-name="Button">
      <div className="bg-clip-padding border-0 border-[transparent] border-solid content-stretch flex flex-col items-center justify-center relative">
        <Container6 />
      </div>
    </div>
  );
}

function Heading() {
  return (
    <div className="relative shrink-0" data-name="Heading 1">
      <div className="bg-clip-padding border-0 border-[transparent] border-solid content-stretch flex flex-col items-center relative">
        <div className="flex flex-col font-['Inter:Semi_Bold',sans-serif] font-semibold h-[28px] justify-center leading-[0] not-italic relative shrink-0 text-[18px] text-[rgba(255,255,255,0.9)] text-center tracking-[-0.45px] w-[118.66px]">
          <p className="leading-[28px] whitespace-pre-wrap">Good Morning</p>
        </div>
      </div>
    </div>
  );
}

function Container8() {
  return (
    <div className="h-[4px] relative shrink-0 w-[16px]" data-name="Container">
      <svg className="absolute block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 16 4">
        <g id="Container">
          <path d={svgPaths.p3a256b80} fill="var(--fill-0, #94A3B8)" id="Icon" />
        </g>
      </svg>
    </div>
  );
}

function ButtonSpacerForBalanceOrOptionalSettingsIcon() {
  return (
    <div className="content-stretch flex items-center justify-center relative shrink-0" data-name="Button - Spacer for balance or optional settings icon">
      <Container8 />
    </div>
  );
}

function Container7() {
  return (
    <div className="relative shrink-0 w-[24px]" data-name="Container">
      <div className="bg-clip-padding border-0 border-[transparent] border-solid content-stretch flex flex-col items-start relative w-full">
        <ButtonSpacerForBalanceOrOptionalSettingsIcon />
      </div>
    </div>
  );
}

function Header() {
  return (
    <div className="backdrop-blur-[6px] bg-[rgba(16,22,34,0.6)] relative shrink-0 w-full z-[3]" data-name="Header">
      <div aria-hidden="true" className="absolute border border-[rgba(255,255,255,0.05)] border-solid inset-0 pointer-events-none" />
      <div className="flex flex-row items-center size-full">
        <div className="content-stretch flex items-center justify-between pl-[25px] pr-[25.02px] py-[17px] relative w-full">
          <Button />
          <Heading />
          <Container7 />
        </div>
      </div>
    </div>
  );
}

function OverlayBorderShadow() {
  return (
    <div className="bg-[rgba(255,255,255,0)] relative rounded-[9999px] shrink-0 size-[32px]" data-name="Overlay+Border+Shadow">
      <div className="content-stretch flex flex-col items-start justify-center overflow-clip p-px relative rounded-[inherit] size-full">
        <div className="flex-[1_0_0] min-h-px min-w-px w-full" data-name="Gradient" style={{ backgroundImage: "linear-gradient(135deg, rgb(99, 102, 241) 0%, rgb(147, 51, 234) 100%)" }} />
      </div>
      <div aria-hidden="true" className="absolute border border-[rgba(255,255,255,0.1)] border-solid inset-0 pointer-events-none rounded-[9999px] shadow-[0px_1px_2px_0px_rgba(0,0,0,0.05)]" />
    </div>
  );
}

function Container10() {
  return (
    <div className="content-stretch flex flex-col items-start relative shrink-0 w-full" data-name="Container">
      <div className="flex flex-col font-['Inter:Regular',sans-serif] font-normal justify-center leading-[0] not-italic relative shrink-0 text-[#94a3b8] text-[12px] w-full">
        <p className="leading-[16px] whitespace-pre-wrap">AI Assistant</p>
      </div>
    </div>
  );
}

function Margin() {
  return (
    <div className="relative shrink-0 w-full" data-name="Margin">
      <div className="content-stretch flex flex-col items-start pl-[4px] relative w-full">
        <Container10 />
      </div>
    </div>
  );
}

function OverlayBorderShadowOverlayBlur() {
  return (
    <div className="backdrop-blur-[6px] bg-[rgba(255,255,255,0.05)] relative rounded-bl-[2px] rounded-br-[16px] rounded-tl-[16px] rounded-tr-[16px] shrink-0 w-full" data-name="Overlay+Border+Shadow+OverlayBlur">
      <div aria-hidden="true" className="absolute border border-[rgba(255,255,255,0.08)] border-solid inset-0 pointer-events-none rounded-bl-[2px] rounded-br-[16px] rounded-tl-[16px] rounded-tr-[16px] shadow-[0px_1px_2px_0px_rgba(0,0,0,0.05)]" />
      <div className="content-stretch flex flex-col items-start pb-[17.255px] pt-[15.875px] px-[17px] relative w-full">
        <div className="flex flex-col font-['Inter:Regular',sans-serif] font-normal h-[74px] justify-center leading-[24.38px] not-italic relative shrink-0 text-[#f1f5f9] text-[15px] w-[265.95px] whitespace-pre-wrap">
          <p className="mb-0">Good morning! I noticed you have a</p>
          <p className="mb-0">packed schedule today. Would you</p>
          <p>like a quick briefing to start your day?</p>
        </div>
      </div>
    </div>
  );
}

function Container9() {
  return (
    <div className="content-stretch flex flex-col gap-[4px] items-start max-w-[304.29998779296875px] relative shrink-0 w-[299.95px]" data-name="Container">
      <Margin />
      <OverlayBorderShadowOverlayBlur />
    </div>
  );
}

function AiMessage() {
  return (
    <div className="content-stretch flex gap-[12px] items-end relative shrink-0 w-full" data-name="AI Message">
      <OverlayBorderShadow />
      <Container9 />
    </div>
  );
}

function AiMessageMargin() {
  return (
    <div className="content-stretch flex flex-col items-start pb-[24px] relative shrink-0 w-full" data-name="AI Message:margin">
      <AiMessage />
    </div>
  );
}

function Margin1() {
  return (
    <div className="content-stretch flex flex-col items-start pr-[4px] relative shrink-0" data-name="Margin">
      <div className="flex flex-col font-['Inter:Regular',sans-serif] font-normal h-[16px] justify-center leading-[0] not-italic relative shrink-0 text-[#94a3b8] text-[12px] w-[21.52px]">
        <p className="leading-[16px] whitespace-pre-wrap">You</p>
      </div>
    </div>
  );
}

function Background() {
  return (
    <div className="bg-[#256af4] content-stretch flex flex-col items-start pb-[16.38px] pt-[15px] px-[16px] relative rounded-bl-[16px] rounded-br-[2px] rounded-tl-[16px] rounded-tr-[16px] shrink-0" data-name="Background">
      <div className="absolute bg-[rgba(255,255,255,0)] inset-0 shadow-[0px_10px_15px_-3px_rgba(37,106,244,0.2),0px_4px_6px_-4px_rgba(37,106,244,0.2)]" data-name="Overlay+Shadow" />
      <div className="flex flex-col font-['Inter:Regular',sans-serif] font-normal h-[25px] justify-center leading-[0] not-italic relative shrink-0 text-[15px] text-white w-[256.2px]">
        <p className="leading-[24.38px] whitespace-pre-wrap">{`Yes please, what's on my schedule?`}</p>
      </div>
    </div>
  );
}

function Container11() {
  return (
    <div className="content-stretch flex flex-col gap-[4px] items-end max-w-[304.29998779296875px] relative shrink-0" data-name="Container">
      <Margin1 />
      <Background />
    </div>
  );
}

function UserMessage() {
  return (
    <div className="content-stretch flex items-end justify-end relative shrink-0 w-full" data-name="User Message">
      <Container11 />
    </div>
  );
}

function UserMessageMargin() {
  return (
    <div className="content-stretch flex flex-col items-start pb-[24px] relative shrink-0 w-full" data-name="User Message:margin">
      <UserMessage />
    </div>
  );
}

function OverlayBorderShadow1() {
  return (
    <div className="bg-[rgba(255,255,255,0)] relative rounded-[9999px] shrink-0 size-[32px]" data-name="Overlay+Border+Shadow">
      <div className="content-stretch flex flex-col items-start justify-center overflow-clip p-px relative rounded-[inherit] size-full">
        <div className="flex-[1_0_0] min-h-px min-w-px w-full" data-name="Gradient" style={{ backgroundImage: "linear-gradient(135deg, rgb(99, 102, 241) 0%, rgb(147, 51, 234) 100%)" }} />
      </div>
      <div aria-hidden="true" className="absolute border border-[rgba(255,255,255,0.1)] border-solid inset-0 pointer-events-none rounded-[9999px] shadow-[0px_1px_2px_0px_rgba(0,0,0,0.05)]" />
    </div>
  );
}

function Container13() {
  return (
    <div className="content-stretch flex flex-col items-start relative shrink-0 w-full" data-name="Container">
      <div className="flex flex-col font-['Inter:Regular',sans-serif] font-normal justify-center leading-[0] not-italic relative shrink-0 text-[#94a3b8] text-[12px] w-full">
        <p className="leading-[16px] whitespace-pre-wrap">AI Assistant</p>
      </div>
    </div>
  );
}

function Margin2() {
  return (
    <div className="relative shrink-0 w-full" data-name="Margin">
      <div className="content-stretch flex flex-col items-start pl-[4px] relative w-full">
        <Container13 />
      </div>
    </div>
  );
}

function Container14() {
  return (
    <div className="relative shrink-0 w-full" data-name="Container">
      <div className="bg-clip-padding border-0 border-[transparent] border-solid content-stretch flex flex-col items-start relative w-full">
        <div className="flex flex-col font-['Inter:Regular',sans-serif] font-normal justify-center leading-[0] not-italic relative shrink-0 text-[#f1f5f9] text-[15px] w-full">
          <p className="leading-[24.38px] whitespace-pre-wrap">Here is your morning summary:</p>
        </div>
      </div>
    </div>
  );
}

function Margin3() {
  return (
    <div className="h-[18.667px] relative shrink-0 w-[15px]" data-name="Margin">
      <svg className="absolute block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 15 18.6667">
        <g id="Margin">
          <path d={svgPaths.p3e493f00} fill="var(--fill-0, #256AF4)" id="Icon" />
        </g>
      </svg>
    </div>
  );
}

function Paragraph() {
  return (
    <div className="content-stretch flex flex-col items-start leading-[0] not-italic pb-[0.82px] relative shrink-0" data-name="Paragraph">
      <div className="flex flex-col font-['Inter:Medium',sans-serif] font-medium h-[25px] justify-center mb-[-0.82px] relative shrink-0 text-[15px] text-white w-[202.84px]">
        <p className="leading-[24.38px] whitespace-pre-wrap">10:00 AM — Product Review</p>
      </div>
      <div className="flex flex-col font-['Inter:Regular',sans-serif] font-normal h-[20px] justify-center mb-[-0.82px] relative shrink-0 text-[#94a3b8] text-[14px] w-[150.5px]">
        <p className="leading-[20px] whitespace-pre-wrap">Zoom Meeting Room B</p>
      </div>
    </div>
  );
}

function Item() {
  return (
    <div className="content-stretch flex gap-[12px] items-start relative shrink-0 w-full" data-name="Item">
      <Margin3 />
      <Paragraph />
    </div>
  );
}

function Margin4() {
  return (
    <div className="h-[18.667px] relative shrink-0 w-[12.5px]" data-name="Margin">
      <svg className="absolute block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 12.5 18.6667">
        <g id="Margin">
          <path d={svgPaths.p3d524200} fill="var(--fill-0, #EAB308)" id="Icon" />
        </g>
      </svg>
    </div>
  );
}

function Paragraph1() {
  return (
    <div className="content-stretch flex flex-col items-start leading-[0] not-italic pb-[0.81px] relative shrink-0" data-name="Paragraph">
      <div className="flex flex-col font-['Inter:Medium',sans-serif] font-medium h-[25px] justify-center mb-[-0.81px] relative shrink-0 text-[15px] text-white w-[213.47px]">
        <p className="leading-[24.38px] whitespace-pre-wrap">12:30 PM — Lunch with Sarah</p>
      </div>
      <div className="flex flex-col font-['Inter:Regular',sans-serif] font-normal h-[20px] justify-center mb-[-0.81px] relative shrink-0 text-[#94a3b8] text-[14px] w-[112.19px]">
        <p className="leading-[20px] whitespace-pre-wrap">Downtown Bistro</p>
      </div>
    </div>
  );
}

function Item1() {
  return (
    <div className="content-stretch flex gap-[12px] items-start relative shrink-0 w-full" data-name="Item">
      <Margin4 />
      <Paragraph1 />
    </div>
  );
}

function List() {
  return (
    <div className="relative shrink-0 w-full" data-name="List">
      <div className="bg-clip-padding border-0 border-[transparent] border-solid content-stretch flex flex-col gap-[11.99px] items-start relative w-full">
        <Item />
        <Item1 />
      </div>
    </div>
  );
}

function HorizontalBorder() {
  return (
    <div className="relative shrink-0 w-full" data-name="HorizontalBorder">
      <div aria-hidden="true" className="absolute border-[rgba(255,255,255,0.05)] border-solid border-t inset-0 pointer-events-none" />
      <div className="bg-clip-padding border-0 border-[transparent] border-solid content-stretch flex flex-col items-start pb-[0.255px] pt-[15.875px] relative w-full">
        <div className="flex flex-col font-['Inter:Regular',sans-serif] font-normal h-[74px] justify-center leading-[24.38px] not-italic relative shrink-0 text-[#cbd5e1] text-[15px] w-[269.64px] whitespace-pre-wrap">
          <p className="mb-0">{`It's currently 72°F and sunny. Should I`}</p>
          <p className="mb-0">order your usual coffee to pick up on</p>
          <p>the way?</p>
        </div>
      </div>
    </div>
  );
}

function OverlayBorderShadowOverlayBlur1() {
  return (
    <div className="backdrop-blur-[6px] bg-[rgba(255,255,255,0.05)] relative rounded-bl-[2px] rounded-br-[16px] rounded-tl-[16px] rounded-tr-[16px] shrink-0 w-full" data-name="Overlay+Border+Shadow+OverlayBlur">
      <div aria-hidden="true" className="absolute border border-[rgba(255,255,255,0.08)] border-solid inset-0 pointer-events-none rounded-bl-[2px] rounded-br-[16px] rounded-tl-[16px] rounded-tr-[16px] shadow-[0px_1px_2px_0px_rgba(0,0,0,0.05)]" />
      <div className="content-stretch flex flex-col gap-[12px] items-start pb-[16.99px] pt-[16px] px-[17px] relative w-full">
        <Container14 />
        <List />
        <HorizontalBorder />
      </div>
    </div>
  );
}

function Container12() {
  return (
    <div className="content-stretch flex flex-col gap-[4px] items-start max-w-[304.29998779296875px] relative shrink-0 w-[304.3px]" data-name="Container">
      <Margin2 />
      <OverlayBorderShadowOverlayBlur1 />
    </div>
  );
}

function AiMessage1() {
  return (
    <div className="content-stretch flex gap-[12px] items-end relative shrink-0 w-full" data-name="AI Message">
      <OverlayBorderShadow1 />
      <Container12 />
    </div>
  );
}

function AiMessageMargin1() {
  return (
    <div className="content-stretch flex flex-col items-start pb-[24px] relative shrink-0 w-full" data-name="AI Message:margin">
      <AiMessage1 />
    </div>
  );
}

function Container16() {
  return (
    <div className="relative shrink-0 size-[16.5px]" data-name="Container">
      <svg className="absolute block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 16.5 16.5">
        <g id="Container">
          <path d={svgPaths.p33c29780} fill="var(--fill-0, #FACC15)" id="Icon" />
        </g>
      </svg>
    </div>
  );
}

function Container17() {
  return (
    <div className="relative shrink-0" data-name="Container">
      <div className="bg-clip-padding border-0 border-[transparent] border-solid content-stretch flex flex-col items-center relative">
        <div className="flex flex-col font-['Inter:Medium',sans-serif] font-medium h-[20px] justify-center leading-[0] not-italic relative shrink-0 text-[#e2e8f0] text-[14px] text-center w-[56.48px]">
          <p className="leading-[20px] whitespace-pre-wrap">Weather</p>
        </div>
      </div>
    </div>
  );
}

function Button1() {
  return (
    <div className="backdrop-blur-[6px] bg-[rgba(255,255,255,0.05)] content-stretch flex gap-[8px] h-[36px] items-center px-[17px] py-px relative rounded-[9999px] shrink-0" data-name="Button">
      <div aria-hidden="true" className="absolute border border-[rgba(255,255,255,0.1)] border-solid inset-0 pointer-events-none rounded-[9999px]" />
      <Container16 />
      <Container17 />
    </div>
  );
}

function Container18() {
  return (
    <div className="h-[13.5px] relative shrink-0 w-[15px]" data-name="Container">
      <svg className="absolute block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 15 13.5">
        <g id="Container">
          <path d={svgPaths.p872c680} fill="var(--fill-0, #60A5FA)" id="Icon" />
        </g>
      </svg>
    </div>
  );
}

function Container19() {
  return (
    <div className="relative shrink-0" data-name="Container">
      <div className="bg-clip-padding border-0 border-[transparent] border-solid content-stretch flex flex-col items-center relative">
        <div className="flex flex-col font-['Inter:Medium',sans-serif] font-medium h-[20px] justify-center leading-[0] not-italic relative shrink-0 text-[#e2e8f0] text-[14px] text-center w-[37.73px]">
          <p className="leading-[20px] whitespace-pre-wrap">News</p>
        </div>
      </div>
    </div>
  );
}

function Button2() {
  return (
    <div className="backdrop-blur-[6px] bg-[rgba(255,255,255,0.05)] content-stretch flex gap-[8px] h-[36px] items-center px-[17px] py-px relative rounded-[9999px] shrink-0" data-name="Button">
      <div aria-hidden="true" className="absolute border border-[rgba(255,255,255,0.1)] border-solid inset-0 pointer-events-none rounded-[9999px]" />
      <Container18 />
      <Container19 />
    </div>
  );
}

function Container20() {
  return (
    <div className="h-[15.038px] relative shrink-0 w-[15px]" data-name="Container">
      <svg className="absolute block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 15 15.0375">
        <g id="Container">
          <path d={svgPaths.p2b228980} fill="var(--fill-0, #F87171)" id="Icon" />
        </g>
      </svg>
    </div>
  );
}

function Container21() {
  return (
    <div className="relative shrink-0" data-name="Container">
      <div className="bg-clip-padding border-0 border-[transparent] border-solid content-stretch flex flex-col items-center relative">
        <div className="flex flex-col font-['Inter:Medium',sans-serif] font-medium h-[20px] justify-center leading-[0] not-italic relative shrink-0 text-[#e2e8f0] text-[14px] text-center w-[71.33px]">
          <p className="leading-[20px] whitespace-pre-wrap">Reminders</p>
        </div>
      </div>
    </div>
  );
}

function Button3() {
  return (
    <div className="backdrop-blur-[6px] bg-[rgba(255,255,255,0.05)] content-stretch flex gap-[8px] h-[36px] items-center px-[17px] py-px relative rounded-[9999px] shrink-0" data-name="Button">
      <div aria-hidden="true" className="absolute border border-[rgba(255,255,255,0.1)] border-solid inset-0 pointer-events-none rounded-[9999px]" />
      <Container20 />
      <Container21 />
    </div>
  );
}

function Container22() {
  return (
    <div className="relative shrink-0 size-[13.5px]" data-name="Container">
      <svg className="absolute block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 13.5 13.5">
        <g id="Container">
          <path d={svgPaths.p34b74c80} fill="var(--fill-0, #4ADE80)" id="Icon" />
        </g>
      </svg>
    </div>
  );
}

function Container23() {
  return (
    <div className="relative shrink-0" data-name="Container">
      <div className="bg-clip-padding border-0 border-[transparent] border-solid content-stretch flex flex-col items-center relative">
        <div className="flex flex-col font-['Inter:Medium',sans-serif] font-medium h-[20px] justify-center leading-[0] not-italic relative shrink-0 text-[#e2e8f0] text-[14px] text-center w-[43.36px]">
          <p className="leading-[20px] whitespace-pre-wrap">Traffic</p>
        </div>
      </div>
    </div>
  );
}

function Button4() {
  return (
    <div className="backdrop-blur-[6px] bg-[rgba(255,255,255,0.05)] content-stretch flex gap-[8px] h-[36px] items-center px-[17px] py-px relative rounded-[9999px] shrink-0" data-name="Button">
      <div aria-hidden="true" className="absolute border border-[rgba(255,255,255,0.1)] border-solid inset-0 pointer-events-none rounded-[9999px]" />
      <Container22 />
      <Container23 />
    </div>
  );
}

function Container15() {
  return (
    <div className="content-stretch flex gap-[12px] items-start relative shrink-0 w-[484.91px]" data-name="Container">
      <Button1 />
      <Button2 />
      <Button3 />
      <Button4 />
    </div>
  );
}

function SuggestedActionsHorizontalScroll() {
  return (
    <div className="relative shrink-0 w-full" data-name="Suggested Actions (Horizontal Scroll)">
      <div className="overflow-clip rounded-[inherit] size-full">
        <div className="content-stretch flex flex-col items-start px-[4px] py-[8px] relative w-full">
          <Container15 />
        </div>
      </div>
    </div>
  );
}

function MainChatContainer() {
  return (
    <div className="max-w-[672px] relative shrink-0 w-full z-[2]" data-name="Main - Chat Container">
      <div className="max-w-[inherit] overflow-clip rounded-[inherit] size-full">
        <div className="content-stretch flex flex-col items-start max-w-[inherit] pb-[169.25px] pt-[16px] px-[16px] relative w-full">
          <div className="h-[16px] shrink-0 w-full" data-name="Welcome Section/Spacer" />
          <AiMessageMargin />
          <UserMessageMargin />
          <AiMessageMargin1 />
          <SuggestedActionsHorizontalScroll />
        </div>
      </div>
    </div>
  );
}

function AmbientBackgroundEffects() {
  return (
    <div className="absolute inset-0 overflow-clip z-[1]" data-name="Ambient Background Effects">
      <div className="absolute bg-[rgba(37,106,244,0.2)] blur-[50px] h-[442px] left-[-39px] rounded-[9999px] top-[-88.39px] w-[195px]" data-name="Overlay+Blur" />
      <div className="absolute bg-[rgba(37,99,235,0.1)] blur-[60px] bottom-[-88.39px] h-[530.39px] right-[-39px] rounded-[9999px] w-[234px]" data-name="Overlay+Blur" />
      <div className="absolute bg-[rgba(99,102,241,0.1)] blur-[40px] h-[265.19px] left-[117px] rounded-[9999px] top-[353.59px] w-[117px]" data-name="Overlay+Blur" />
    </div>
  );
}

export default function Body() {
  return (
    <div className="bg-[#101622] content-stretch flex flex-col isolate items-start relative size-full" data-name="Body">
      <FloatingInputArea />
      <Header />
      <MainChatContainer />
      <AmbientBackgroundEffects />
    </div>
  );
}