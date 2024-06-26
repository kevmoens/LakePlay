﻿// <auto-generated />
using TriviaForCheeseHeads.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace TriviaForCheeseHeads.Migrations
{
    [DbContext(typeof(TriviaForCheeseHeadsSqliteContext))]
    [Migration("20240303205651_AddIdentityColumns")]
    partial class AddIdentityColumns
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.27");

            modelBuilder.Entity("TriviaForCheeseHeads.Data.TriviaQuestion", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("AskedThisRound")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Difficulty")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Used")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("TriviaForCheeseHeads.Data.TriviaQuestionOption", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsAnswer")
                        .HasColumnType("INTEGER");

                    b.Property<string>("QuestionId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TriviaQuestionId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("TriviaQuestionId");

                    b.ToTable("TriviaQuestionOption");
                });

            modelBuilder.Entity("TriviaForCheeseHeads.Data.TriviaQuestionOption", b =>
                {
                    b.HasOne("TriviaForCheeseHeads.Data.TriviaQuestion", null)
                        .WithMany("ListOptions")
                        .HasForeignKey("TriviaQuestionId");
                });

            modelBuilder.Entity("TriviaForCheeseHeads.Data.TriviaQuestion", b =>
                {
                    b.Navigation("ListOptions");
                });
#pragma warning restore 612, 618
        }
    }
}
