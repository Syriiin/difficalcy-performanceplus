FROM alpine:3

RUN apk add --no-cache bash curl jq

WORKDIR /e2e

COPY test .

CMD [ "./test" ]
