ARG GAMECI_IMAGE=unityci/editor:ubuntu-6000.0.17f1-base-3.1.0
FROM $GAMECI_IMAGE

ARG BLENDER_SHORT_VERSION=3.4
ARG BLENDER_FULL_VERSION=3.4.1

RUN echo "BLENDER_SHORT_VERSION: $BLENDER_SHORT_VERSION"
RUN echo "BLENDER_FULL_VERSION: $BLENDER_FULL_VERSION"
RUN apt-get update && \
    apt-get install -y wget && \
    wget https://download.blender.org/release/Blender$BLENDER_SHORT_VERSION/blender-$BLENDER_FULL_VERSION-linux-x64.tar.xz && \
    tar -xf blender-$BLENDER_FULL_VERSION-linux-x64.tar.xz && \
    rm blender-$BLENDER_FULL_VERSION-linux-x64.tar.xz

ENV PATH="$PATH:/blender-$BLENDER_FULL_VERSION-linux-x64"
