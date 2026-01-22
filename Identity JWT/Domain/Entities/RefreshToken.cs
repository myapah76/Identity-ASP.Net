using Domain.Commons;
using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class RefreshToken : IEntity
{
    public long Id { get; set; }

    public string Token { get; set; } = null!;

    public DateTimeOffset? Issued_at { get; set; }

    public DateTimeOffset Expires_at { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public bool? Is_revoked { get; set; }

    public long User_id { get; set; }

    public virtual User User { get; set; } = null!;
}
