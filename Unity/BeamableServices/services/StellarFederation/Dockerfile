ARG BEAM_DOTNET_VERSION="8.0-noble"
FROM mcr.microsoft.com/dotnet/runtime:${BEAM_DOTNET_VERSION}

# Install all required native dependencies in a single layer:
# - libicu74 → globalization support for .NET
# - libc6, libstdc++6, libgcc-s1 → core runtime libraries
# - libdbus-1-3, unzip → Stellar CLI dependencies
# - build-essential, curl → needed for libsodium build and Rust install
ARG LIBSODIUM_VERSION="1.0.20"
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        libicu74 \
        libc6 \
        libstdc++6 \
        libgcc-s1 \
        pkg-config \
        libssl-dev \
        libdbus-1-3 \
        unzip \
        build-essential \
        curl && \
    curl -sS https://download.libsodium.org/libsodium/releases/libsodium-${LIBSODIUM_VERSION}.tar.gz | tar xz && \
    cd libsodium-${LIBSODIUM_VERSION} && \
    ./configure && make && make install && \
    ldconfig && \
    cd .. && rm -rf libsodium-${LIBSODIUM_VERSION} && \
    rm -rf /var/lib/apt/lists/*

# Install Rust
ARG RUST_VERSION=1.90.0
RUN curl https://sh.rustup.rs -sSf | sh -s -- -y --default-toolchain ${RUST_VERSION} \
    && echo 'source $HOME/.cargo/env' >> /root/.bashrc
ENV PATH="/root/.cargo/bin:${PATH}"
RUN rustup target add wasm32v1-none

# These args are provided by the Beam CLI

# Declares the relative path from the beamable workspace to the pre-build support binaries for StellarFederation
#  Normally, this will be /services/StellarFederation/bin/beamApp/support
ARG BEAM_SUPPORT_SRC_PATH

# Declares the relative path from the beamable workspace to the pre-built binaries for StellarFederation
#  Normally, this will be /services/StellarFederation/bin/beamApp/app
ARG BEAM_APP_SRC_PATH

# Declares where the built application will exist inside the Docker image.
#  This value is usually /beamApp/StellarFederation
ARG BEAM_APP_DEST

# <beamReserved> Beamable may inject custom settings into this Dockerfile. Please do not remove these lines.
# </beamReserved>

# /beamApp is the directory that will hold the application
WORKDIR /beamApp

# expose the health port
EXPOSE 6565

# copy general supporting files
COPY $BEAM_SUPPORT_SRC_PATH .

# copy specific application code
COPY $BEAM_APP_SRC_PATH .

# Extract Stellar CLI
ENV STELLAR_PATH="/usr/local/bin/stellar"
RUN unzip -o Tools/stellar.zip -d /usr/local/bin/ && chmod +x ${STELLAR_PATH}

# ensure that the application is runnable
RUN chmod +x $BEAM_APP_DEST
ENV BEAM_APP=$BEAM_APP_DEST

# when starting the container, run dotnet with the built dll
ENTRYPOINT "dotnet" $BEAM_APP

# Swap entrypoints if the container is exploding and you want to keep it alive indefinitely so you can go look into it.
#ENTRYPOINT ["tail", "-f", "/dev/null"]