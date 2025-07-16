FROM node:18-alpine

WORKDIR /app

COPY package.json package-lock.json* ./

RUN npm install

RUN npm i -g serve

# Copy source files
COPY . .

# Set build-time environment variable
ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL

# Build the application
RUN npm run build

EXPOSE 3000

# Serve on port 3000 explicitly
CMD ["serve", "-s", "dist", "-l", "3000"]