# Clone stage
FROM alpine/git AS clone
WORKDIR /app

# Configure git
RUN git config --global http.postBuffer 157286400 && \
    git config --global core.compression 9

# Clone with depth limit and retry logic
RUN git clone --depth=1 --progress https://github.com/RomanC451/TrackYourLife.git . || \
    (sleep 5 && git clone --depth=1 --progress --single-branch https://github.com/RomanC451/TrackYourLife.git .)

# Build stage
FROM node:18-alpine AS build
WORKDIR /app
COPY --from=clone /app/ClientApp .
RUN npm ci
RUN npm run build

# Runtime stage
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 5173

# Configure nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf
CMD ["nginx", "-g", "daemon off;"]