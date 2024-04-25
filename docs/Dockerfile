FROM python:3.12-alpine3.19

RUN apk add --no-cache tini

WORKDIR /docs

COPY requirements.txt .

RUN pip install --no-cache-dir -r requirements.txt

COPY . .

EXPOSE 8000

ENTRYPOINT [ "/sbin/tini", "--", "mkdocs" ]

CMD [ "serve", "--dev-addr=0.0.0.0:8000"]
