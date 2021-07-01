﻿using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LinkDotNet.Domain;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence.Sql
{
    public sealed class SqlRepositoryTests : SqlDatabaseTestBase
    {
        [Fact]
        public async Task ShouldLoadBlogPost()
        {
            var blogPost = BlogPost.Create("Title", "Subtitle", "Content", "url", new[] { "Tag 1", "Tag 2" });
            await DbContext.BlogPosts.AddAsync(blogPost);
            await DbContext.SaveChangesAsync();

            var blogPostFromRepo = await BlogPostRepository.GetByIdAsync(blogPost.Id);

            blogPostFromRepo.Should().NotBeNull();
            blogPostFromRepo.Title.Should().Be("Title");
            blogPostFromRepo.ShortDescription.Should().Be("Subtitle");
            blogPostFromRepo.Content.Should().Be("Content");
            blogPostFromRepo.PreviewImageUrl.Should().Be("url");
            blogPostFromRepo.Tags.Should().HaveCount(2);
            var tagContent = blogPostFromRepo.Tags.Select(t => t.Content).ToList();
            tagContent.Should().Contain(new[] { "Tag 1", "Tag 2" });
        }

        [Fact]
        public async Task ShouldSaveBlogPost()
        {
            var blogPost = BlogPost.Create("Title", "Subtitle", "Content", "url", new[] { "Tag 1", "Tag 2" });

            await BlogPostRepository.StoreAsync(blogPost);

            var blogPostFromContext = await DbContext.BlogPosts.Include(b => b.Tags).AsNoTracking().SingleOrDefaultAsync(s => s.Id == blogPost.Id);
            blogPostFromContext.Should().NotBeNull();
            blogPostFromContext.Title.Should().Be("Title");
            blogPostFromContext.ShortDescription.Should().Be("Subtitle");
            blogPostFromContext.Content.Should().Be("Content");
            blogPostFromContext.PreviewImageUrl.Should().Be("url");
            blogPostFromContext.Tags.Should().HaveCount(2);
            var tagContent = blogPostFromContext.Tags.Select(t => t.Content).ToList();
            tagContent.Should().Contain(new[] { "Tag 1", "Tag 2" });
        }

        [Fact]
        public async Task ShouldGetAllBlogPosts()
        {
            var blogPost = BlogPost.Create("Title", "Subtitle", "Content", "url", new[] { "Tag 1", "Tag 2" });
            await DbContext.BlogPosts.AddAsync(blogPost);
            await DbContext.SaveChangesAsync();

            var blogPostsFromRepo = (await BlogPostRepository.GetAllAsync()).ToList();

            blogPostsFromRepo.Should().NotBeNull();
            blogPostsFromRepo.Should().HaveCount(1);
            var blogPostFromRepo = blogPostsFromRepo.Single();
            blogPostFromRepo.Title.Should().Be("Title");
            blogPostFromRepo.ShortDescription.Should().Be("Subtitle");
            blogPostFromRepo.Content.Should().Be("Content");
            blogPostFromRepo.PreviewImageUrl.Should().Be("url");
            blogPostFromRepo.Tags.Should().HaveCount(2);
            var tagContent = blogPostFromRepo.Tags.Select(t => t.Content).ToList();
            tagContent.Should().Contain(new[] { "Tag 1", "Tag 2" });
        }
    }
}