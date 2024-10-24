FROM python:3.12-alpine3.19

RUN apk add --no-cache tini

RUN pip install poetry=="1.8.3"

WORKDIR /docs

COPY pyproject.toml poetry.lock ./

RUN poetry install --no-cache

COPY . .

EXPOSE 8000

ENTRYPOINT [ "/sbin/tini", "--", "poetry", "run", "mkdocs" ]

CMD [ "serve", "--dev-addr=0.0.0.0:8000"]
