FROM node:alpine as builder
WORKDIR /app
COPY . .
RUN npm ci 
RUN npm run build

FROM nginx:alpine as production
ENV NODE_ENV production
COPY --from=builder /app/build /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]



# FROM node:latest
# WORKDIR /app
# COPY package*.json ./
# RUN npm install
# COPY . .
# EXPOSE 55559
# CMD ["npm", "start"]