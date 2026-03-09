FROM mcr.microsoft.com/dotnet/sdk:8.0

# Install dependencies
RUN apt-get update && apt-get install -y \
    wget \
    unzip \
    openjdk-17-jdk \
    && rm -rf /var/lib/apt/lists/*

# Set Java home
ENV JAVA_HOME=/usr/lib/jvm/java-17-openjdk-amd64

# Install Android SDK
ENV ANDROID_HOME=/opt/android-sdk
ENV PATH="${PATH}:${ANDROID_HOME}/cmdline-tools/latest/bin:${ANDROID_HOME}/platform-tools"

RUN mkdir -p ${ANDROID_HOME}/cmdline-tools && \
    wget -q https://dl.google.com/android/repository/commandlinetools-linux-11076708_latest.zip -O /tmp/cmdline-tools.zip && \
    unzip -q /tmp/cmdline-tools.zip -d ${ANDROID_HOME}/cmdline-tools && \
    mv ${ANDROID_HOME}/cmdline-tools/cmdline-tools ${ANDROID_HOME}/cmdline-tools/latest && \
    rm /tmp/cmdline-tools.zip

# Accept licenses and install packages
RUN yes | ${ANDROID_HOME}/cmdline-tools/latest/bin/sdkmanager --licenses && \
    ${ANDROID_HOME}/cmdline-tools/latest/bin/sdkmanager \
    "platform-tools" \
    "platforms;android-34" \
    "build-tools;34.0.0" \
    "ndk;26.1.10909125"

# Install Uno Platform templates
RUN dotnet new install Uno.Templates

WORKDIR /app

# Copy project files
COPY . .

# Build APK
RUN dotnet publish VIRA.Mobile/VIRA.Mobile.csproj \
    -f net8.0-android \
    -c Release \
    -p:AndroidSdkDirectory=${ANDROID_HOME} \
    -o /app/output

# Output will be in /app/output
CMD ["echo", "Build complete! APK is in /app/output"]
