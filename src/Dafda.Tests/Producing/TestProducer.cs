﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Dafda.Tests.TestDoubles;
using Xunit;

namespace Dafda.Tests.Producing
{
    public class TestProducer
    {
        [Fact]
        public async Task Can_produce_message()
        {
            var spy = new KafkaProducerSpy();

            var sut = A.Producer
                .With(spy)
                .With(A.OutgoingMessageRegistry
                    .Register<Message>("foo", "bar", @event => @event.Id)
                    .Build()
                )
                .Build();

            await sut.Produce(new Message {Id = "dummyId"});

            Assert.Equal("foo", spy.Topic);
            Assert.Equal("dummyId", spy.Key);
        }

        public class Message
        {
            public string Id { get; set; }
        }

        [Fact]
        public async Task produces_message_to_expected_topic()
        {
            var spy = new KafkaProducerSpy();

            var expectedTopic = "foo";

            var sut = A.Producer
                .With(spy)
                .With(A.OutgoingMessageRegistry
                    .Register<Message>(expectedTopic, "bar", @event => @event.Id)
                    .Build()
                )
                .Build();

            await sut.Produce(
                message: new Message { Id = "dummyId" },
                headers: new Dictionary<string, object>
                {
                    {"foo-key", "foo-value"}
                }
            );

            Assert.Equal(expectedTopic, spy.Topic);
        }

        [Fact]
        public async Task produces_message_with_expected_partition_key()
        {
            var spy = new KafkaProducerSpy();

            var expectedKey = "foo-partition-key";

            var sut = A.Producer
                .With(spy)
                .With(A.OutgoingMessageRegistry
                    .Register<Message>("foo", "bar", @event => @event.Id)
                    .Build()
                )
                .Build();

            await sut.Produce(
                message: new Message { Id = expectedKey },
                headers: new Dictionary<string, object>
                {
                    {"foo-key", "foo-value"}
                }
            );

            Assert.Equal(expectedKey, spy.Key);
        }

        [Fact]
        public async Task produces_message_with_expected_serialized_value()
        {
            var expectedValue = "foo-value";
            
            var spy = new KafkaProducerSpy(new PayloadSerializerStub(expectedValue));

            var sut = A.Producer
                .With(spy)
                .With(A.OutgoingMessageRegistry
                    .Register<Message>("foo", "bar", @event => @event.Id)
                    .Build()
                )
                .Build();

            await sut.Produce(
                message: new Message { Id = "dummyId" },
                headers: new Dictionary<string, object>
                {
                    {"foo-key", "foo-value"}
                }
            );

            Assert.Equal(expectedValue, spy.Value);
        }

        [Fact]
        public async Task produces_expected_message_without_headers_using_default_serializer()
        {
            var spy = new KafkaProducerSpy();

            var sut = A.Producer
                .With(spy)
                .With(new MessageIdGeneratorStub(() => "1"))
                .With(A.OutgoingMessageRegistry
                    .Register<Message>("foo", "bar", @event => @event.Id)
                    .Build()
                )
                .Build();

            await sut.Produce(
                message: new Message { Id = "dummyId" },
                headers: new Dictionary<string, object>
                {
                }
            );

            var expectedValue = @"{""messageId"":""1"",""type"":""bar"",""data"":{""id"":""dummyId""}}";

            Assert.Equal(expectedValue, spy.Value);
        }

        [Fact]
        public async Task produces_expected_message_with_headers_using_default_serializer()
        {
            var spy = new KafkaProducerSpy();

            var sut = A.Producer
                .With(spy)
                .With(new MessageIdGeneratorStub(() => "1"))
                .With(A.OutgoingMessageRegistry
                    .Register<Message>("foo", "bar", @event => @event.Id)
                    .Build()
                )
                .Build();

            await sut.Produce(
                message: new Message { Id = "dummyId" },
                headers: new Dictionary<string, object>
                {
                    {"foo-key", "foo-value"}
                }
            );

            var expectedValue = @"{""messageId"":""1"",""type"":""bar"",""foo-key"":""foo-value"",""data"":{""id"":""dummyId""}}";

            Assert.Equal(expectedValue, spy.Value);
        }
    }
}