FROM alpine:3

RUN apk add --no-cache bash curl diffutils jq

WORKDIR /e2e

COPY test snapshot.txt ./

CMD [ "./test" ]
