FROM node:21-alpine3.18

WORKDIR /ClientApp

COPY package.json .

RUN npm install --legacy-peer-deps

COPY . .

EXPOSE 5173

CMD [ "npm", "run", "dev" ]