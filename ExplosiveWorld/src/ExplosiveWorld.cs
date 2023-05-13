using BepInEx;
using System;
using UnityEngine;
using Menu.Remix.MixedUI;
using static Player;
using Random = UnityEngine.Random;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using DevInterface;
using System.Drawing;
using Color = UnityEngine.Color;
using static MonoMod.InlineRT.MonoModRule;
using static MoreSlugcats.SingularityBomb;

namespace ExplosiveWorld;
[BepInPlugin("ExplosiveWorld", "Explosive World", "0.1")]

public class ExplosiveWorld : BaseUnityPlugin {

    static bool _initialized;

    private readonly float bombDelay = 5f;
    private float bombTime = 0f;

    private readonly float bombPolelDelay = 0.6f;
    private float bombPoleTimer = 0f;

    private readonly float bombKelplDelay = 0.6f;
    private float bombKelpTimer = 0f;

    private bool bombInit = true;
    private bool bombJumped = false;

    private bool grabbombInit = true;
    private bool grabbombInit2 = true;
    private bool grabbombInit3 = true;

    private bool bombSnail = true;

    public Creature killTag;

    public static ExplosiveMenu optionsMenuInstance;

    private void LogInfo(object ex) => Logger.LogInfo(ex);

    public void OnEnable() {
        LogInfo("Explosive world is working!");
        On.RainWorld.OnModsInit += RainWorld_OnModsInit;
    }

    private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self) {
        orig(self);

        try
        {
            if (_initialized) return;
            _initialized = true;

            On.Player.Update += Player_Update;

            On.DropBug.Jump += DropBug_Jump;
            On.DropBug.Update += DropBug_Update;

            On.WaterNut.Swell += WaterNut_Swell;

            On.Snail.Click += Snail_Click;

            MachineConnector.SetRegisteredOI("ExplosiveWorld", optionsMenuInstance = new ExplosiveMenu());

        }
        catch (Exception ex)
        {
            Debug.Log($"Remix Menu: Hook_OnModsInit options failed init error {optionsMenuInstance}{ex}");
            Logger.LogError(ex);
            Logger.LogMessage("WHOOPS something go wrong");
        }
        finally
        {
            orig.Invoke(self);
        }
    }

    private void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        var room = self.room;
        var pos = self.firstChunk.pos;
        var color = self.ShortCutColor();

        orig(self, eu);

        if (self.dead || self.room is null || self is null)
        {
            bombInit = true;
            grabbombInit = true;
            grabbombInit2 = true;
            grabbombInit3 = true;

            bombJumped = false;

            bombTime = 0f;
            bombPoleTimer = 0f;
            bombKelpTimer = 0f;
        }

        if (bombJumped && !self.dead && !ExplosiveMenu.NoBombs.Value)
        {
            bombTime += Time.deltaTime;
        }

        for (int dropbug = 0; dropbug < self.grabbedBy.Count; dropbug++)
        {
            if (self?.grabbedBy?[dropbug].grabber is DropBug && grabbombInit && !ExplosiveMenu.NoBombs.Value && !ExplosiveMenu.SingulatiryBombs.Value)
            {
                room?.AddObject(new Explosion(room, self, pos, 7, 250f, 6.2f, 2f, 280f, 0.25f, killTag, 0.7f, 160f, 1f));
                room?.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
                room?.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
                room?.AddObject(new ExplosionSpikes(room, pos, 14, 30f, 9f, 7f, 170f, color));
                room?.AddObject(new ShockWave(pos, 330f, 0.045f, 5, false));
                room?.PlaySound(SoundID.Bomb_Explode, pos);

                self.Die();
                grabbombInit = false;
            }

            if (self?.grabbedBy?[dropbug].grabber is DropBug && grabbombInit && ExplosiveMenu.SingulatiryBombs.Value && !ExplosiveMenu.NoBombs.Value)
            {
                room?.AddObject(new SparkFlash(pos, 300f, new Color(0f, 0f, 1f)));
                room?.AddObject(new Explosion(room, self, pos, 7, 250f, 6.2f, 200f, 280f, 0.25f, killTag, 0.7f, 160f, 1f));
                room?.AddObject(new Explosion(room, self, pos, 7, 2000f, 4f, 220f, 400f, 0.25f, killTag, 0.3f, 200f, 1f));
                room?.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
                room?.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
                room?.AddObject(new Explosion.ExplosionLight(pos, 2000f, 2f, 60, color));
                room?.AddObject(new ShockWave(pos, 350f, 0.485f, 300, highLayer: true));
                room?.AddObject(new ShockWave(pos, 2000f, 0.185f, 180));
                room?.PlaySound(SoundID.Bomb_Explode, pos);
                room?.InGameNoise(new Noise.InGameNoise(pos, 9000f, self, 1f));

                self.Die();
                grabbombInit = false;
            }
        }

        for (int polemimic = 0; polemimic < self.grabbedBy.Count; polemimic++)
        {
            if (self?.grabbedBy?[polemimic].grabber is PoleMimic && !ExplosiveMenu.NoBombs.Value)
            {
                bombPoleTimer += Time.deltaTime;
            }

            if (self?.grabbedBy?[polemimic].grabber is PoleMimic && grabbombInit2 && bombPoleTimer >= bombPolelDelay && !ExplosiveMenu.NoBombs.Value && !ExplosiveMenu.SingulatiryBombs.Value)
            {
                room?.AddObject(new Explosion(room, self, pos, 7, 250f, 6.2f, 2f, 280f, 0.25f, killTag, 0.7f, 160f, 1f));
                room?.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
                room?.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
                room?.AddObject(new ExplosionSpikes(room, pos, 14, 30f, 9f, 7f, 170f, color));
                room?.AddObject(new ShockWave(pos, 330f, 0.045f, 5, false));
                room?.PlaySound(SoundID.Bomb_Explode, pos);

                self.Die();
                grabbombInit2 = false;
                bombPoleTimer = 0;
            }

            if (self?.grabbedBy?[polemimic].grabber is PoleMimic && grabbombInit2 && bombPoleTimer >= bombPolelDelay && ExplosiveMenu.SingulatiryBombs.Value && !ExplosiveMenu.NoBombs.Value)
            {
                room?.AddObject(new SparkFlash(pos, 300f, new Color(0f, 0f, 1f)));
                room?.AddObject(new Explosion(room, self, pos, 7, 250f, 6.2f, 200f, 280f, 0.25f, killTag, 0.7f, 160f, 1f));
                room?.AddObject(new Explosion(room, self, pos, 7, 2000f, 4f, 220f, 400f, 0.25f, killTag, 0.3f, 200f, 1f));
                room?.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
                room?.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
                room?.AddObject(new Explosion.ExplosionLight(pos, 2000f, 2f, 60, color));
                room?.AddObject(new ShockWave(pos, 350f, 0.485f, 300, highLayer: true));
                room?.AddObject(new ShockWave(pos, 2000f, 0.185f, 180));
                room?.PlaySound(SoundID.Bomb_Explode, pos);
                room?.InGameNoise(new Noise.InGameNoise(pos, 9000f, self, 1f));

                self.Die();
                grabbombInit2 = false;
                bombPoleTimer = 0;
            }
        }

        for (int kelpmonster = 0; kelpmonster < self.grabbedBy.Count; kelpmonster++)
        {
            if (self?.grabbedBy?[kelpmonster].grabber is TentaclePlant && !ExplosiveMenu.NoBombs.Value)
            {
                bombKelpTimer += Time.deltaTime;
            }

            if (self?.grabbedBy?[kelpmonster].grabber is TentaclePlant && grabbombInit3 && bombKelpTimer >= bombKelplDelay && !ExplosiveMenu.NoBombs.Value && !ExplosiveMenu.SingulatiryBombs.Value)
            {
                room?.AddObject(new Explosion(room, self, pos, 7, 250f, 6.2f, 2f, 280f, 0.25f, killTag, 0.7f, 160f, 1f));
                room?.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
                room?.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
                room?.AddObject(new ExplosionSpikes(room, pos, 14, 30f, 9f, 7f, 170f, color));
                room?.AddObject(new ShockWave(pos, 330f, 0.045f, 5, false));
                room?.PlaySound(SoundID.Bomb_Explode, pos);

                self.Die();
                grabbombInit3 = false;
                bombKelpTimer = 0f;
            }

            if (self?.grabbedBy?[kelpmonster].grabber is TentaclePlant && grabbombInit3 && bombKelpTimer >= bombKelplDelay && ExplosiveMenu.SingulatiryBombs.Value && !ExplosiveMenu.NoBombs.Value)
            {
                room?.AddObject(new SparkFlash(pos, 300f, new Color(0f, 0f, 1f)));
                room?.AddObject(new Explosion(room, self, pos, 7, 250f, 6.2f, 200f, 280f, 0.25f, killTag, 0.7f, 160f, 1f));
                room?.AddObject(new Explosion(room, self, pos, 7, 2000f, 4f, 220f, 400f, 0.25f, killTag, 0.3f, 200f, 1f));
                room?.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
                room?.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
                room?.AddObject(new Explosion.ExplosionLight(pos, 2000f, 2f, 60, color));
                room?.AddObject(new ShockWave(pos, 350f, 0.485f, 300, highLayer: true));
                room?.AddObject(new ShockWave(pos, 2000f, 0.185f, 180));
                room?.PlaySound(SoundID.Bomb_Explode, pos);
                room?.InGameNoise(new Noise.InGameNoise(pos, 9000f, self, 1f));

                self.Die();
                grabbombInit3 = false;
                bombKelpTimer = 0f;
            }
        }
    }

    private void DropBug_Jump(On.DropBug.orig_Jump orig, DropBug self, Vector2 jumpDir)
    {
        var room = self.room;
        var pos = self.firstChunk.pos;
        var color = self.ShortCutColor();

        orig(self, jumpDir);
        if (self.jumping && bombInit && self.room is not null && self is not null && !self.dead && !ExplosiveMenu.NoBombs.Value && !ExplosiveMenu.SingulatiryBombs.Value)
        {
            room?.AddObject(new Explosion(room, self, pos, 7, 50f, 6.2f, 2f, 280f, 0.25f, killTag, 0.7f, 160f, 1f));
            room?.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
            room?.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
            room?.AddObject(new ExplosionSpikes(room, pos, 14, 30f, 9f, 7f, 170f, color));
            room?.AddObject(new ShockWave(pos, 330f, 0.045f, 5, false));
            room?.PlaySound(SoundID.Bomb_Explode, pos);

            bombInit = false;
            bombJumped = true;
        }

        if (self.jumping && bombInit && self.room is not null && self is not null && !self.dead && ExplosiveMenu.SingulatiryBombs.Value && !ExplosiveMenu.NoBombs.Value)
        {
            room?.AddObject(new SparkFlash(pos, 300f, new Color(0f, 0f, 1f)));
            room?.AddObject(new Explosion(room, self, pos, 7, 250f, 6.2f, 200f, 280f, 0.25f, killTag, 0.7f, 160f, 1f));
            room?.AddObject(new Explosion(room, self, pos, 7, 2000f, 4f, 220f, 400f, 0.25f, killTag, 0.3f, 200f, 1f));
            room?.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
            room?.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
            room?.AddObject(new Explosion.ExplosionLight(pos, 2000f, 2f, 60, color));
            room?.AddObject(new ShockWave(pos, 350f, 0.485f, 300, highLayer: true));
            room?.AddObject(new ShockWave(pos, 2000f, 0.185f, 180));
            room?.PlaySound(SoundID.Bomb_Explode, pos);
            room?.InGameNoise(new Noise.InGameNoise(pos, 9000f, self, 1f));

            bombInit = false;
            bombJumped = true;
        }
    }

    private void DropBug_Update(On.DropBug.orig_Update orig, DropBug self, bool eu)
    {
        var room = self.room;
        var pos = self.firstChunk.pos;
        var color = self.ShortCutColor();

        orig(self, eu);

        if (bombTime >= bombDelay && !self.dead && !ExplosiveMenu.NoBombs.Value && !ExplosiveMenu.SingulatiryBombs.Value)
        {
            room?.AddObject(new Explosion(room, self, pos, 7, 50f, 6.2f, 2f, 280f, 0.25f, killTag, 0.7f, 160f, 1f));
            room?.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
            room?.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
            room?.AddObject(new ExplosionSpikes(room, pos, 14, 30f, 9f, 7f, 170f, color));
            room?.AddObject(new ShockWave(pos, 330f, 0.045f, 5, false));
            room?.PlaySound(SoundID.Bomb_Explode, pos);

            self.Die();
            bombTime = 0f;
            bombJumped = false;
        }

        if (bombTime >= bombDelay && !self.dead && bombSnail && ExplosiveMenu.SingulatiryBombs.Value && !ExplosiveMenu.NoBombs.Value)
        {
            room?.AddObject(new SparkFlash(pos, 300f, new Color(0f, 0f, 1f)));
            room?.AddObject(new Explosion(room, self, pos, 7, 250f, 6.2f, 200f, 280f, 0.25f, killTag, 0.7f, 160f, 1f));
            room?.AddObject(new Explosion(room, self, pos, 7, 2000f, 4f, 220f, 400f, 0.25f, killTag, 0.3f, 200f, 1f));
            room?.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
            room?.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
            room?.AddObject(new Explosion.ExplosionLight(pos, 2000f, 2f, 60, color));
            room?.AddObject(new ShockWave(pos, 350f, 0.485f, 300, highLayer: true));
            room?.AddObject(new ShockWave(pos, 2000f, 0.185f, 180));
            room?.PlaySound(SoundID.Bomb_Explode, pos);
            room?.InGameNoise(new Noise.InGameNoise(pos, 9000f, self, 1f));

            self.Die();
            bombTime = 0f;
            bombJumped = false;
        }
    }

    private void Snail_Click(On.Snail.orig_Click orig, Snail self)
    {
        var room = self.room;
        var pos = self.firstChunk.pos;
        var color = self.ShortCutColor();

        orig(self);
        if (self.triggered)
        {
            bombSnail = true;
        }

        if (!self.triggered && self.room is not null && self is not null && bombSnail && !ExplosiveMenu.NoBombs.Value && !ExplosiveMenu.SingulatiryBombs.Value)
        {
            room?.AddObject(new Explosion(room, self, pos, 7, 250f, 6.2f, 2f, 280f, 0.25f, killTag, 0.7f, 160f, 1f));
            room?.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
            room?.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
            room?.AddObject(new ExplosionSpikes(room, pos, 14, 30f, 9f, 7f, 170f, color));
            room?.AddObject(new ShockWave(pos, 330f, 0.045f, 5, false));
            room?.PlaySound(SoundID.Bomb_Explode, pos);

            bombSnail = false;
        }

        if (!self.triggered && self.room is not null && self is not null && bombSnail && ExplosiveMenu.SingulatiryBombs.Value && !ExplosiveMenu.NoBombs.Value)
        {
            room?.AddObject(new SparkFlash(pos, 300f, new Color(0f, 0f, 1f)));
            room?.AddObject(new Explosion(room, self, pos, 7, 250f, 6.2f, 200f, 280f, 0.25f, killTag, 0.7f, 160f, 1f));
            room?.AddObject(new Explosion(room, self, pos, 7, 2000f, 4f, 220f, 400f, 0.25f, killTag, 0.3f, 200f, 1f));
            room?.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
            room?.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
            room?.AddObject(new Explosion.ExplosionLight(pos, 2000f, 2f, 60, color));
            room?.AddObject(new ShockWave(pos, 350f, 0.485f, 300, highLayer: true));
            room?.AddObject(new ShockWave(pos, 2000f, 0.185f, 180));
            room?.PlaySound(SoundID.Bomb_Explode, pos);
            room?.InGameNoise(new Noise.InGameNoise(pos, 9000f, self, 1f));

            bombSnail = false;
        }
    } 

    private void WaterNut_Swell(On.WaterNut.orig_Swell orig, WaterNut self)
    {
        var room = self.room;
        var pos = self.firstChunk.pos;
        var color = new Color(1f, 1f, 1f);

        orig(self);
        if (self.AbstrNut.swollen && self.room is not null && self is not null && !ExplosiveMenu.NoBombs.Value && !ExplosiveMenu.SingulatiryBombs.Value)
        {
            room?.AddObject(new Explosion(room, self, pos, 7, 250f, 6.2f, 2f, 280f, 0.25f, killTag, 0.7f, 160f, 1f));
            room?.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
            room?.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
            room?.AddObject(new ExplosionSpikes(room, pos, 14, 30f, 9f, 7f, 170f, color));
            room?.AddObject(new ShockWave(pos, 330f, 0.045f, 5, false));
            room?.PlaySound(SoundID.Bomb_Explode, pos);
        }

        if (self.AbstrNut.swollen && self.room is not null && self is not null && ExplosiveMenu.SingulatiryBombs.Value && !ExplosiveMenu.NoBombs.Value)
        {
            room?.AddObject(new SparkFlash(pos, 300f, new Color(0f, 0f, 1f)));
            room?.AddObject(new Explosion(room, self, pos, 7, 250f, 6.2f, 200f, 280f, 0.25f, killTag, 0.7f, 160f, 1f));
            room?.AddObject(new Explosion(room, self, pos, 7, 2000f, 4f, 220f, 400f, 0.25f, killTag, 0.3f, 200f, 1f));
            room?.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
            room?.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
            room?.AddObject(new Explosion.ExplosionLight(pos, 2000f, 2f, 60, color));
            room?.AddObject(new ShockWave(pos, 350f, 0.485f, 300, highLayer: true));
            room?.AddObject(new ShockWave(pos, 2000f, 0.185f, 180));
            room?.PlaySound(SoundID.Bomb_Explode, pos);
            room?.InGameNoise(new Noise.InGameNoise(pos, 9000f, self, 1f));
        }    
    }
}