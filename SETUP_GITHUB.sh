#!/bin/bash

# Script untuk setup GitHub repository dan trigger build otomatis

echo "🚀 VIRA GitHub Setup Script"
echo "==========================="
echo ""
echo "Script ini akan:"
echo "  1. Initialize Git repository"
echo "  2. Add all files"
echo "  3. Create initial commit"
echo "  4. Siap untuk push ke GitHub"
echo ""

# Check if git is installed
if ! command -v git &> /dev/null; then
    echo "❌ Git not installed. Please install git first:"
    echo "   sudo apt-get install git"
    exit 1
fi

echo "✅ Git found: $(git --version)"
echo ""

# Initialize git if not already
if [ ! -d ".git" ]; then
    echo "📦 Initializing Git repository..."
    git init
    echo "✅ Git initialized"
else
    echo "✅ Git repository already exists"
fi

# Configure git if not configured
if [ -z "$(git config user.name)" ]; then
    echo ""
    echo "⚙️  Git Configuration"
    read -p "Enter your name: " git_name
    read -p "Enter your email: " git_email
    
    git config user.name "$git_name"
    git config user.email "$git_email"
    echo "✅ Git configured"
fi

echo ""
echo "📝 Adding files to git..."
git add .

echo ""
echo "💾 Creating commit..."
git commit -m "Initial commit: VIRA AI Assistant

- Uno Platform Android app
- Gemini AI integration
- Voice input/output support
- Modern glassmorphism UI
- GitHub Actions auto-build setup"

echo ""
echo "✅ Repository ready!"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "📋 NEXT STEPS:"
echo ""
echo "1. Create GitHub repository:"
echo "   → Open: https://github.com/new"
echo "   → Repository name: vira-android-app"
echo "   → Description: VIRA - AI Assistant for Android"
echo "   → Public or Private (your choice)"
echo "   → DON'T initialize with README"
echo "   → Click 'Create repository'"
echo ""
echo "2. Copy the repository URL (will look like):"
echo "   https://github.com/YOUR_USERNAME/vira-android-app.git"
echo ""
echo "3. Run these commands:"
echo ""
echo "   git remote add origin https://github.com/YOUR_USERNAME/vira-android-app.git"
echo "   git branch -M main"
echo "   git push -u origin main"
echo ""
echo "4. After push, GitHub Actions will automatically:"
echo "   ✓ Build APK (~10-15 minutes)"
echo "   ✓ Upload to Artifacts"
echo ""
echo "5. Download APK:"
echo "   → Go to: https://github.com/YOUR_USERNAME/vira-android-app/actions"
echo "   → Click latest workflow run"
echo "   → Scroll down to 'Artifacts'"
echo "   → Download 'VIRA-Release-APK'"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "💡 TIP: Bookmark this for future updates:"
echo "   https://github.com/YOUR_USERNAME/vira-android-app/actions"
echo ""
echo "🎉 Happy coding!"
