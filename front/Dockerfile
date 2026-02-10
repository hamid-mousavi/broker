# FROM node:18-alpine
# WORKDIR /app

# COPY Front/package*.json ./
# RUN npm install

# COPY Front/ .

# EXPOSE 5173
# ENV HOST=0.0.0.0

# CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0", "--port", "5173"]
FROM node:18-alpine
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .

EXPOSE 80
ENV HOST=0.0.0.0

CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0", "--port", "5173"]

