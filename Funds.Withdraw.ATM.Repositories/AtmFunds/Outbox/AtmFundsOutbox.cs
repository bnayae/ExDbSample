using EvDbSample.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvDbSample.Repositories;

[EvDbAttachMessageType<AtmFundsDepositedMessage>]
[EvDbOutbox<AtmFundsFactory>]
internal partial class AtmFundsOutbox
{
    protected override void ProduceOutboxMessages(AtmFundsDepositedEvent payload,
                                                  IEvDbEventMeta meta,
                                                  EvDbAtmFundsViews views,
                                                  AtmFundsOutboxContext outbox)
    {
        base.ProduceOutboxMessages(payload, meta, views, outbox);
        AtmFundsDepositedMessage msg = new(payload.Data);
        outbox.Add(msg);
    }
}
