using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ModManager.database.generate;

public partial class GameContext : DbContext
{
    public GameContext()
    {
    }

    public GameContext(DbContextOptions<GameContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Block> Blocks { get; set; }

    public virtual DbSet<Chunk> Chunks { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<World> Worlds { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=127.0.0.1;Database=game;Username=postgres;Password=example");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Block>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("primary_pk");

            entity.ToTable("block");

            entity.HasIndex(e => new { e.BlockName, e.ModName }, "block_pk").IsUnique();

            entity.HasIndex(e => e.State, "block_state_index");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BlockName)
                .HasMaxLength(100)
                .HasColumnName("block_name");
            entity.Property(e => e.ModName)
                .HasMaxLength(100)
                .HasColumnName("mod_name");
            entity.Property(e => e.State)
                .HasDefaultValue(1)
                .HasComment("方块状态，如是否被禁用")
                .HasColumnName("state");
        });

        modelBuilder.Entity<Chunk>(entity =>
        {
            entity.HasKey(e => new { e.PosX, e.WorldId, e.PosZ, e.PosY }).HasName("chunk_pk");

            entity.ToTable("chunk", tb => tb.HasComment("区块数据"));

            entity.Property(e => e.PosX)
                .HasComment("X轴坐标")
                .HasColumnName("pos_x");
            entity.Property(e => e.WorldId).HasColumnName("world_id");
            entity.Property(e => e.PosZ)
                .HasComment("z轴坐标")
                .HasColumnName("pos_z");
            entity.Property(e => e.PosY)
                .HasComment("y轴坐标")
                .HasColumnName("pos_y");
            entity.Property(e => e.Data)
                .HasComment("区块数据")
                .HasColumnName("data");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.SlotKey }).HasName("inventory_pk");

            entity.ToTable("inventory", tb => tb.HasComment("背包"));

            entity.Property(e => e.UserId)
                .HasComment("用户id")
                .HasColumnName("user_id");
            entity.Property(e => e.SlotKey)
                .HasMaxLength(200)
                .HasComment("背包类型")
                .HasColumnName("slot_key");
            entity.Property(e => e.SlotValue)
                .HasComment("背包内容")
                .HasColumnName("slot_value");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_info_pk");

            entity.ToTable("user", tb => tb.HasComment("玩家注册表"));

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("主键")
                .HasColumnName("id");
            entity.Property(e => e.GameMode)
                .HasDefaultValue(0)
                .HasComment("游戏模式")
                .HasColumnName("game_mode");
            entity.Property(e => e.Nickname)
                .HasMaxLength(200)
                .HasComment("显示用的昵称")
                .HasColumnName("nickname");
            entity.Property(e => e.Password)
                .HasMaxLength(64)
                .HasComment("用户密码散列值")
                .HasColumnName("password");
            entity.Property(e => e.PosX)
                .HasDefaultValue(0L)
                .HasComment("x坐标")
                .HasColumnName("pos_x");
            entity.Property(e => e.PosY)
                .HasDefaultValue(0L)
                .HasComment("y坐标")
                .HasColumnName("pos_y");
            entity.Property(e => e.PosZ)
                .HasDefaultValue(0L)
                .HasComment("z坐标")
                .HasColumnName("pos_z");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasComment("登录所用的用户名")
                .HasColumnName("username");
            entity.Property(e => e.WorldId)
                .HasDefaultValue(0L)
                .HasComment("所在世界id")
                .HasColumnName("world_id");
        });

        modelBuilder.Entity<World>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("world_pk");

            entity.ToTable("world");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("世界id")
                .HasColumnName("id");
            entity.Property(e => e.ActiveTime)
                .HasComment("最后激活时间")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("active_time");
            entity.Property(e => e.CreateTime)
                .HasComment("创建时间")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_time");
            entity.Property(e => e.Name)
                .HasComment("世界名称")
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
