using Android.Animation;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using VIRA.Mobile.SharedModels;
using VIRA.Mobile.SharedServices;

namespace VIRA.Mobile.Views;

/// <summary>
/// Chat history sidebar that slides in from the left
/// </summary>
public class ChatHistorySidebar : FrameLayout
{
    private const string TAG = "ChatHistorySidebar";
    private const int ANIMATION_DURATION_MS = 300;
    
    private readonly LinearLayout _sidebarContainer;
    private readonly RecyclerView _conversationList;
    private readonly Button _newChatButton;
    private readonly Button _clearHistoryButton;
    private readonly TextView _emptyStateText;
    private readonly ConversationManager _conversationManager;
    
    private ConversationListAdapter? _adapter;
    private bool _isVisible = false;
    
    public interface ISidebarListener
    {
        void OnConversationSelected(string conversationId);
        void OnNewChatRequested();
        void OnClearHistoryRequested();
    }
    
    private ISidebarListener? _listener;

    public ChatHistorySidebar(Context context, ConversationManager conversationManager) : base(context)
    {
        _conversationManager = conversationManager ?? throw new ArgumentNullException(nameof(conversationManager));
        
        // Main container - full screen overlay
        LayoutParameters = new FrameLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.MatchParent);
        SetBackgroundColor(Android.Graphics.Color.Transparent);
        Visibility = ViewStates.Gone;
        
        // Sidebar container
        _sidebarContainer = new LinearLayout(context)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new FrameLayout.LayoutParams(
                (int)(Resources?.DisplayMetrics?.WidthPixels * 0.8 ?? 800),
                ViewGroup.LayoutParams.MatchParent)
        };
        
        var sidebarDrawable = new GradientDrawable();
        sidebarDrawable.SetColor(Android.Graphics.Color.ParseColor("#1A1F2E"));
        _sidebarContainer.Background = sidebarDrawable;
        _sidebarContainer.SetPadding(32, 48, 32, 32);
        
        // Header with buttons
        var headerLayout = new LinearLayout(context)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                BottomMargin = 32
            }
        };
        
        var titleText = new TextView(context)
        {
            Text = "Chat History",
            TextSize = 20,
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1
            }
        };
        titleText.SetTextColor(Android.Graphics.Color.White);
        titleText.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        headerLayout.AddView(titleText);
        
        _sidebarContainer.AddView(headerLayout);
        
        // New Chat button
        _newChatButton = new Button(context)
        {
            Text = "+ New Chat",
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                BottomMargin = 16
            }
        };
        var newChatDrawable = new GradientDrawable();
        newChatDrawable.SetColor(Android.Graphics.Color.ParseColor("#8B5CF6"));
        newChatDrawable.SetCornerRadius(24);
        _newChatButton.Background = newChatDrawable;
        _newChatButton.SetTextColor(Android.Graphics.Color.White);
        _newChatButton.SetAllCaps(false);
        _newChatButton.SetPadding(0, 32, 0, 32);
        _newChatButton.Click += OnNewChatClick;
        _sidebarContainer.AddView(_newChatButton);
        
        // RecyclerView for conversations
        _conversationList = new RecyclerView(context)
        {
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                0)
            {
                Weight = 1
            }
        };
        _conversationList.SetLayoutManager(new LinearLayoutManager(context));
        _sidebarContainer.AddView(_conversationList);
        
        // Empty state
        _emptyStateText = new TextView(context)
        {
            Text = "No conversations yet\nStart a new chat!",
            TextSize = 14,
            Gravity = GravityFlags.Center,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                0)
            {
                Weight = 1
            }
        };
        _emptyStateText.SetTextColor(Android.Graphics.Color.ParseColor("#64748B"));
        _emptyStateText.Visibility = ViewStates.Gone;
        _sidebarContainer.AddView(_emptyStateText);
        
        // Clear History button
        _clearHistoryButton = new Button(context)
        {
            Text = "Clear History",
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 16
            }
        };
        var clearDrawable = new GradientDrawable();
        clearDrawable.SetColor(Android.Graphics.Color.ParseColor("#EF4444"));
        clearDrawable.SetCornerRadius(24);
        _clearHistoryButton.Background = clearDrawable;
        _clearHistoryButton.SetTextColor(Android.Graphics.Color.White);
        _clearHistoryButton.SetAllCaps(false);
        _clearHistoryButton.SetPadding(0, 32, 0, 32);
        _clearHistoryButton.Click += OnClearHistoryClick;
        _sidebarContainer.AddView(_clearHistoryButton);
        
        AddView(_sidebarContainer);
        
        // Initially position sidebar off-screen to the left
        _sidebarContainer.TranslationX = -(_sidebarContainer.LayoutParameters as FrameLayout.LayoutParams)?.Width ?? -800;
    }

    public void SetListener(ISidebarListener listener)
    {
        _listener = listener;
    }

    public void Show()
    {
        if (_isVisible) return;
        
        Visibility = ViewStates.Visible;
        _isVisible = true;
        
        // Ensure hardware acceleration is enabled for smooth animations
        if (_sidebarContainer.LayerType != LayerType.Hardware)
        {
            _sidebarContainer.SetLayerType(LayerType.Hardware, null);
        }
        
        // Animate sidebar sliding in using hardware-accelerated properties
        // TranslationX and Alpha are GPU-accelerated for 60fps performance
        _sidebarContainer.Animate()
            .TranslationX(0)
            .SetDuration(ANIMATION_DURATION_MS)
            .SetInterpolator(new Android.Views.Animations.DecelerateInterpolator())
            .WithEndAction(new Java.Lang.Runnable(() => 
            {
                // Reset to software layer after animation for memory efficiency
                _sidebarContainer.SetLayerType(LayerType.None, null);
            }))
            .Start();
        
        // Fade in background
        Animate()
            .Alpha(1f)
            .SetDuration(ANIMATION_DURATION_MS)
            .Start();
        
        RefreshConversations();
        
        Android.Util.Log.Info(TAG, "Sidebar shown");
    }

    public void Hide()
    {
        if (!_isVisible) return;
        
        _isVisible = false;
        
        var sidebarWidth = (_sidebarContainer.LayoutParameters as FrameLayout.LayoutParams)?.Width ?? 800;
        
        // Ensure hardware acceleration is enabled for smooth animations
        if (_sidebarContainer.LayerType != LayerType.Hardware)
        {
            _sidebarContainer.SetLayerType(LayerType.Hardware, null);
        }
        
        // Animate sidebar sliding out using hardware-accelerated properties
        _sidebarContainer.Animate()
            .TranslationX(-sidebarWidth)
            .SetDuration(ANIMATION_DURATION_MS)
            .SetInterpolator(new Android.Views.Animations.AccelerateInterpolator())
            .WithEndAction(new Java.Lang.Runnable(() => 
            {
                Visibility = ViewStates.Gone;
                // Reset to software layer after animation
                _sidebarContainer.SetLayerType(LayerType.None, null);
            }))
            .Start();
        
        // Fade out background
        Animate()
            .Alpha(0f)
            .SetDuration(ANIMATION_DURATION_MS)
            .Start();
        
        Android.Util.Log.Info(TAG, "Sidebar hidden");
    }

    public void RefreshConversations()
    {
        var conversations = _conversationManager.LoadAllConversations();
        
        if (conversations.Count == 0)
        {
            _conversationList.Visibility = ViewStates.Gone;
            _emptyStateText.Visibility = ViewStates.Visible;
        }
        else
        {
            _conversationList.Visibility = ViewStates.Visible;
            _emptyStateText.Visibility = ViewStates.Gone;
            
            _adapter = new ConversationListAdapter(Context!, conversations, OnConversationClick);
            _conversationList.SetAdapter(_adapter);
        }
        
        Android.Util.Log.Info(TAG, $"Refreshed conversations: {conversations.Count} items");
    }

    private void OnNewChatClick(object? sender, EventArgs e)
    {
        _listener?.OnNewChatRequested();
        Hide();
    }

    private void OnClearHistoryClick(object? sender, EventArgs e)
    {
        _listener?.OnClearHistoryRequested();
    }

    private void OnConversationClick(string conversationId)
    {
        _listener?.OnConversationSelected(conversationId);
        Hide();
    }

    public override bool OnTouchEvent(MotionEvent? e)
    {
        if (e?.Action == MotionEventActions.Down)
        {
            // Check if touch is outside sidebar
            var sidebarWidth = (_sidebarContainer.LayoutParameters as FrameLayout.LayoutParams)?.Width ?? 800;
            if (e.GetX() > sidebarWidth)
            {
                Hide();
                return true;
            }
        }
        return base.OnTouchEvent(e);
    }
}
