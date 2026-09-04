FROM node:24-alpine

WORKDIR /app

COPY Aspotus.Admin.Web/package.json Aspotus.Admin.Web/package-lock.json ./
RUN npm ci --no-audit --no-fund

COPY Aspotus.Admin.Web/ ./

EXPOSE 5174

CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0", "--port", "5174"]
