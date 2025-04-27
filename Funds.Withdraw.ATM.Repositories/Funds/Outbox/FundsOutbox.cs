using EvDbSample.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvDbSample.Repositories;

[EvDbAttachMessageType<FundsDepositedMessage>]
[EvDbAttachMessageType<AtmFundsDepositedMessage>]
[EvDbOutbox<FundsFactory>]
internal partial class FundsOutbox
{
    protected override void ProduceOutboxMessages(FundsDepositRequestedEvent payload,
                                                  IEvDbEventMeta meta,
                                                  EvDbFundsViews views,
                                                  FundsOutboxContext outbox)
    {
        FundsDepositedMessage msg = new(payload.Data, payload.Source)
        {
            Segments  = payload.Segments
        };
        outbox.Add(msg);
    }
}
