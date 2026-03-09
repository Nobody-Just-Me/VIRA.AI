using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using VIRA.Mobile.SharedModels;

namespace VIRA.Mobile.Views;

/// <summary>
/// RecyclerView adapter for displaying conversation list with DiffUtil support
/// </summary>
public class ConversationListAdapter : RecyclerView.Adapter
{
    private readonly Context _context;
    private List<Conversation> _conversations;
    private readonly Action<string> _onItemClick;

    public ConversationListAdapter(Context context, List<Conversation> conversations, Action<string> onItemClick)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _conversations = conversations ?? throw new ArgumentNullException(nameof(conversations));
        _onItemClick = onItemClick ?? throw new ArgumentNullException(nameof(onItemClick));
    }

    public override int ItemCount => _conversations.Count;
    
    /// <summary>
    /// Updates the conversation list using DiffUtil for efficient updates
    /// </summary>
    public void UpdateConversations(List<Conversation> newConversations)
    {
        var diffCallback = new ConversationDiffCallback(_conversations, newConversations);
        var diffResult = DiffUtil.CalculateDiff(diffCallback);
        
        _conversations = newConversations;
        diffResult.DispatchUpdatesTo(this);
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var itemView = CreateConversationItemView(_context);
        return new ConversationViewHolder(itemView, _onItemClick);
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        if (holder is ConversationViewHolder viewHolder)
        {
            var conversation = _conversations[position];
            viewHolder.Bind(conversation);
        }
    }

    private LinearLayout CreateConversationItemView(Context context)
    {
        var itemLayout = new LinearLayout(context)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };
        
        var cardDrawable = new GradientDrawable();
        cardDrawable.SetColor(Android.Graphics.Color.ParseColor("#0DFFFFFF"));
        cardDrawable.SetStroke(2, Android.Graphics.Color.ParseColor("#1AFFFFFF"));
        cardDrawable.SetCornerRadius(32);
        itemLayout.Background = cardDrawable;
        itemLayout.SetPadding(48, 32, 48, 32);
        
        // Add ripple effect
        var rippleDrawable = new RippleDrawable(
            Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.ParseColor("#1AFFFFFF")),
            cardDrawable,
            null);
        itemLayout.Background = rippleDrawable;
        
        // Set margin
        var layoutParams = new LinearLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent)
        {
            BottomMargin = 16
        };
        itemLayout.LayoutParameters = layoutParams;
        
        // Title
        var titleText = new TextView(context)
        {
            TextSize = 16,
            Ellipsize = Android.Text.TextUtils.TruncateAt.End,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };
        titleText.SetTextColor(Android.Graphics.Color.White);
        titleText.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        titleText.SetMaxLines(2);
        itemLayout.AddView(titleText);
        
        // Info row (timestamp and message count)
        var infoRow = new LinearLayout(context)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 8
            }
        };
        
        var timestampText = new TextView(context)
        {
            TextSize = 12,
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1
            }
        };
        timestampText.SetTextColor(Android.Graphics.Color.ParseColor("#94A3B8"));
        infoRow.AddView(timestampText);
        
        var messageCountText = new TextView(context)
        {
            TextSize = 12,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };
        messageCountText.SetTextColor(Android.Graphics.Color.ParseColor("#94A3B8"));
        infoRow.AddView(messageCountText);
        
        itemLayout.AddView(infoRow);
        
        return itemLayout;
    }

    private class ConversationViewHolder : RecyclerView.ViewHolder
    {
        private readonly LinearLayout _itemLayout;
        private readonly TextView _titleText;
        private readonly TextView _timestampText;
        private readonly TextView _messageCountText;
        private readonly Action<string> _onItemClick;
        private string? _conversationId;

        public ConversationViewHolder(View itemView, Action<string> onItemClick) : base(itemView)
        {
            _itemLayout = (LinearLayout)itemView;
            _titleText = (TextView)_itemLayout.GetChildAt(0);
            var infoRow = (LinearLayout)_itemLayout.GetChildAt(1);
            _timestampText = (TextView)infoRow.GetChildAt(0);
            _messageCountText = (TextView)infoRow.GetChildAt(1);
            _onItemClick = onItemClick;
            
            _itemLayout.Click += OnClick;
        }

        public void Bind(Conversation conversation)
        {
            _conversationId = conversation.Id;
            _titleText.Text = conversation.GetDisplayTitle();
            _timestampText.Text = conversation.GetFormattedTimestamp();
            _messageCountText.Text = $"{conversation.GetMessageCount()} messages";
        }

        private void OnClick(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_conversationId))
            {
                _onItemClick(_conversationId);
            }
        }
    }
    
    /// <summary>
    /// DiffUtil callback for efficient list updates
    /// </summary>
    private class ConversationDiffCallback : DiffUtil.Callback
    {
        private readonly List<Conversation> _oldList;
        private readonly List<Conversation> _newList;

        public ConversationDiffCallback(List<Conversation> oldList, List<Conversation> newList)
        {
            _oldList = oldList;
            _newList = newList;
        }

        public override int OldListSize => _oldList.Count;
        public override int NewListSize => _newList.Count;

        public override bool AreItemsTheSame(int oldItemPosition, int newItemPosition)
        {
            return _oldList[oldItemPosition].Id == _newList[newItemPosition].Id;
        }

        public override bool AreContentsTheSame(int oldItemPosition, int newItemPosition)
        {
            var oldItem = _oldList[oldItemPosition];
            var newItem = _newList[newItemPosition];
            
            return oldItem.Title == newItem.Title &&
                   oldItem.UpdatedAt == newItem.UpdatedAt &&
                   oldItem.Messages.Count == newItem.Messages.Count;
        }
    }
}
