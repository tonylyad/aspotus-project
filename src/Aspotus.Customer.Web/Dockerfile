FROM node:24-alpine

WORKDIR /app

COPY Aspotus.Customer.Web/package.json Aspotus.Customer.Web/package-lock.json ./
RUN npm ci --no-audit --no-fund

COPY Aspotus.Customer.Web/ ./

EXPOSE 5173

CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0", "--port", "5173"]
