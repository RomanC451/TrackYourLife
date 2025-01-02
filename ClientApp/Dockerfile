# Build stage
FROM node:18-alpine AS build
WORKDIR /app
COPY ClientApp/package*.json ./
RUN npm ci
COPY ClientApp/ .
RUN npm run build

# Runtime stage
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 5173

# Configure nginx
COPY ClientApp/nginx.conf /etc/nginx/conf.d/default.conf
CMD ["nginx", "-g", "daemon off;"]
