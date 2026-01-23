using IdentityService.Domain.Commons;
using System;
using System.Collections.Generic;

namespace IdentityService.Domain.Entities;

public partial class RefreshToken : IEntity
{
    public Guid Id { get; set; }

    public string Token { get; set; } = null!;

    public DateTimeOffset? Issued_at { get; set; }

    public DateTimeOffset Expires_at { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public bool? Is_revoked { get; set; }

    public Guid User_id { get; set; }

    public virtual User User { get; set; } = null!;
}
