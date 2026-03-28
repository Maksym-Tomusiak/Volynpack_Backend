﻿namespace Application.Common.Interfaces.Services.BackgroundEmail;

public record EmailMessage(
    string ToEmail,
    string Subject,
    string Body,
    bool IsHtml,
    string? UnsubscribeUrl = null,
    bool IsSubscribe = false);
