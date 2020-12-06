﻿namespace Alphacloud.MessagePack.AspNetCore.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::MessagePack;
    using Internal;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Mvc.Formatters;


    /// <summary>
    ///     MVC Input formatter.
    /// </summary>
    [PublicAPI]
    public class MessagePackInputFormatter : InputFormatter
    {
        readonly ReadableTypesCache _readableTypesCache;
        readonly MessagePackSerializerOptions _options;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="options">Formatter options.</param>
        /// <param name="mediaTypes">Supported media types.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="options" /> or <paramref name="mediaTypes" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="mediaTypes" /> collection is empty.</exception>
        public MessagePackInputFormatter(MessagePackSerializerOptions options, ICollection<string> mediaTypes)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            if (mediaTypes == null) throw new ArgumentNullException(nameof(mediaTypes));
            if (mediaTypes.Count == 0) throw new ArgumentException("Media type must be specified.", nameof(mediaTypes));
            _readableTypesCache = new ReadableTypesCache(options.Resolver);

            foreach (var mediaType in mediaTypes)
            {
                SupportedMediaTypes.Add(mediaType);
            }
        }

        /// <inheritdoc />
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var httpContext = context.HttpContext;
            var result = await MessagePackSerializer.DeserializeAsync(context.ModelType, httpContext.Request.Body, _options,
                httpContext.RequestAborted).ConfigureAwait(false);
            // ReSharper disable once MethodHasAsyncOverload
            return InputFormatterResult.Success(result);
        }

        /// <inheritdoc />
        /// <exception cref="T:System.ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        protected override bool CanReadType(Type type)
        {
            return _readableTypesCache.CanRead(type);
        }

    }
}
