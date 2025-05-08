#!/bin/bash

# Create SNS topic
awslocal sns create-topic --name local-topic

# Create SQS queue
awslocal sqs create-queue --queue-name local-queue

# Get the ARN of the SQS queue
QUEUE_ARN=$(awslocal sqs get-queue-attributes --queue-url http://localhost:4566/000000000000/local-queue --attribute-names QueueArn --query 'Attributes.QueueArn' --output text)

# Subscribe the SQS queue to the SNS topic
awslocal sns subscribe --topic-arn arn:aws:sns:us-east-1:000000000000:local-topic --protocol sqs --notification-endpoint $QUEUE_ARN

# Allow SNS to send messages to the SQS queue by setting the appropriate policy
awslocal sqs set-queue-attributes --queue-url http://localhost:4566/000000000000/local-queue --attributes '{"Policy":"{\"Version\":\"2012-10-17\",\"Statement\":[{\"Effect\":\"Allow\",\"Principal\":\"*\",\"Action\":\"sqs:SendMessage\",\"Resource\":\"'"$QUEUE_ARN"'\",\"Condition\":{\"ArnEquals\":{\"aws:SourceArn\":\"arn:aws:sns:us-east-1:000000000000:local-topic\"}}}]}"}'
