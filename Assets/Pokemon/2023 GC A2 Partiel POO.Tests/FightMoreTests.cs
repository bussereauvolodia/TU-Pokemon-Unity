using _2023_GC_A2_Partiel_POO.Level_2;
using NUnit.Framework;
using System;

namespace _2023_GC_A2_Partiel_POO.Tests.Level_2
{
    public class FightMoreTests
    {
        // Tu as probablement remarqué qu'il y a encore beaucoup de code qui n'a pas été testé ...
        // À présent c'est à toi de créer les TU sur le reste et de les implémenter
        
        // Ce que tu peux ajouter:
        // - Ajouter davantage de sécurité sur les tests apportés
            // - un heal ne régénère pas plus que les HP Max
            // - si on abaisse les HPMax les HP courant doivent suivre si c'est au dessus de la nouvelle valeur
            // - ajouter un equipement qui rend les attaques prioritaires puis l'enlever et voir que l'attaque n'est plus prioritaire etc)
        // - Le support des status (sleep et burn) qui font des effets à la fin du tour et/ou empeche le pkmn d'agir
        // - Gérer la notion de force/faiblesse avec les différentes attaques à disposition (skills.cs) //Fait
        // - Cumuler les force/faiblesses en ajoutant un type pour l'équipement qui rendrait plus sensible/résistant à un type

        
        [Test]
        public void HealPokemon()
        {
            Character salameche = new Character(100, 50, 40, 20, TYPE.FIRE);
            Punch p = new Punch();
            salameche.ReceiveAttack(p);
            Assert.That(salameche.CurrentHealth, Is.EqualTo(70));
            salameche.Heal(10);
            Assert.That(salameche.CurrentHealth, Is.EqualTo(80));
            salameche.Heal(20);
            Assert.That(salameche.CurrentHealth, Is.EqualTo(100));
        }

        [Test]

        public void HealPokemonWithMaxHealth()
        {
            Character salameche = new Character(100, 50, 40, 20, TYPE.FIRE);
            salameche.Heal(20);
            Assert.That(salameche.CurrentHealth, Is.EqualTo(100));
            Punch p = new Punch();
            salameche.ReceiveAttack(p);
            salameche.Heal(100);
            Assert.That(salameche.CurrentHealth, Is.EqualTo(100));
        }
        
        [Test]

        public void HealPokemonWithEquipment()
        {
            Character salameche = new Character(100, 50, 40, 20, TYPE.FIRE);
            var healthBoost = new Equipment(20, 0, 0, 0);
            salameche.Equip(healthBoost);
            salameche.Heal(20);
            Assert.That(salameche.CurrentHealth, Is.EqualTo(120));
            Punch p = new Punch();
            salameche.ReceiveAttack(p);
            salameche.Heal(100);
            Assert.That(salameche.CurrentHealth, Is.EqualTo(120));
        }
        
        [Test]
        public void TypeResolverWorking()
        {
            Assert.That(TypeResolver.GetFactor(TYPE.FIRE,TYPE.NORMAL), Is.EqualTo(1));
            Assert.That(TypeResolver.GetFactor(TYPE.NORMAL,TYPE.NORMAL), Is.EqualTo(1));
            Assert.That(TypeResolver.GetFactor(TYPE.FIRE,TYPE.GRASS), Is.EqualTo(1.2f));
            Assert.That(TypeResolver.GetFactor(TYPE.GRASS,TYPE.FIRE), Is.EqualTo(0.8f));
            Assert.That(TypeResolver.GetFactor(TYPE.GRASS,TYPE.WATER), Is.EqualTo(1.2f));
            Assert.That(TypeResolver.GetFactor(TYPE.WATER,TYPE.WATER), Is.EqualTo(1));

        }
        
        
        [Test]
        public void FireTypeRecievingWaterAttack()
        {
            Character salameche = new Character(100, 50, 10, 20, TYPE.FIRE);
            WaterBlouBlou w = new WaterBlouBlou();
            salameche.ReceiveAttack(w);
            Assert.That(salameche.CurrentHealth, Is.EqualTo(86));
        }
        
        [Test]
        public void FightTypeCheckNormalAttacks()
        {
            Character salameche = new Character(100, 50, 30, 20, TYPE.FIRE);
            Character bulbizarre = new Character(90, 60, 10, 200, TYPE.GRASS);
            Fight f = new Fight(salameche, bulbizarre);
            Punch p = new Punch();

            // Both uses punch
            f.ExecuteTurn(p, p);

            Assert.That(salameche.IsAlive, Is.EqualTo(true));
            Assert.That(bulbizarre.IsAlive, Is.EqualTo(true));
            Assert.That(salameche.CurrentHealth, Is.EqualTo(salameche.MaxHealth-(p.Power-salameche.Defense)));
            Assert.That(bulbizarre.CurrentHealth, Is.EqualTo(bulbizarre.MaxHealth-(p.Power-bulbizarre.Defense)));
        }
        
        [Test]
        public void FightTypeCheckTypedAttacks()
        {
            Character salameche = new Character(100, 50, 30, 20, TYPE.FIRE);
            Character bulbizarre = new Character(90, 60, 10, 200, TYPE.GRASS);
            Fight f = new Fight(salameche, bulbizarre);
            FireBall fi = new FireBall();
            MagicalGrass m = new MagicalGrass();

            // Salameche and bulbizarre uses FireBall and MagicalGrass
            f.ExecuteTurn(fi, m);

            int fireAttackPower = (int)( fi.Power * TypeResolver.GetFactor(fi.Type, bulbizarre.BaseType));
            int grassAttackPower = (int)( m.Power * TypeResolver.GetFactor(m.Type, salameche.BaseType));
            Assert.That(salameche.IsAlive, Is.EqualTo(true));
            Assert.That(bulbizarre.IsAlive, Is.EqualTo(true));
            Assert.That(salameche.CurrentHealth, Is.EqualTo(salameche.MaxHealth-(grassAttackPower -salameche.Defense)));
            Assert.That(bulbizarre.CurrentHealth, Is.EqualTo(bulbizarre.MaxHealth-(fireAttackPower-bulbizarre.Defense)));
        }
        
        [Test]
        public void PokemonDefaultLevelCheck()
        {
            Character salameche = new Character(100, 50, 30, 20, TYPE.FIRE);
            Assert.That(salameche.Level.LevelAmount, Is.EqualTo(1));
            Assert.That(salameche.Level.CurrentXP, Is.EqualTo(0));
            Assert.That(salameche.Level.RequiredXP, Is.EqualTo(100));
        }
        
        [Test]
        public void PokemonAddXPCheck()
        {
            Character salameche = new Character(100, 50, 30, 20, TYPE.FIRE);
            Assert.That(salameche.Level.CurrentXP, Is.EqualTo(0));
            salameche.Level.AddXP(10);
            Assert.That(salameche.Level.CurrentXP, Is.EqualTo(10));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                salameche.Level.AddXP(-10);;
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                salameche.Level.AddXP(0);;
            });
            Assert.That(salameche.Level.CurrentXP, Is.EqualTo(10));
            salameche.Level.AddXP(70);
            Assert.That(salameche.Level.CurrentXP, Is.EqualTo(80));

        }
        
        [Test]
        public void PokemonLevelUpCheck()
        {
            Character salameche = new Character(100, 50, 30, 20, TYPE.FIRE);
            salameche.Level.AddXP(100);
            Assert.That(salameche.Level.CurrentXP, Is.EqualTo(0));
            Assert.That(salameche.Level.LevelAmount, Is.EqualTo(2));
            Assert.That(salameche.Level.RequiredXP, Is.EqualTo(200));

        }
        
        [Test]
        public void PokemonTwoLevelUpCheck()
        {
            Character salameche = new Character(100, 50, 30, 20, TYPE.FIRE);
            salameche.Level.AddXP(100);
            salameche.Level.AddXP(100);
            Assert.That(salameche.Level.CurrentXP, Is.EqualTo(100));
            Assert.That(salameche.Level.LevelAmount, Is.EqualTo(2));
            salameche.Level.AddXP(100);
            Assert.That(salameche.Level.CurrentXP, Is.EqualTo(0));
            Assert.That(salameche.Level.LevelAmount, Is.EqualTo(3));
        }
        
        [Test]
        public void PokemonConserveExtraXPAfterLevelUp()
        {
            Character salameche = new Character(100, 50, 30, 20, TYPE.FIRE);
            salameche.Level.AddXP(120);
            Assert.That(salameche.Level.LevelAmount, Is.EqualTo(2));
            Assert.That(salameche.Level.CurrentXP, Is.EqualTo(20));
        }
        
        [Test]
        public void WinXPAfterFight()
        {
            Character salameche = new Character(100, 50, 30, 20, TYPE.FIRE);
            Character bulbizarre = new Character(90, 60, 10, 200, TYPE.GRASS);
            Fight f = new Fight(salameche, bulbizarre);
            MegaPunch mp = new MegaPunch();
            Punch p = new Punch();
            f.ExecuteTurn(mp,p);
            Assert.That(salameche.Level.CurrentXP, Is.EqualTo(50));
        }
        
        [Test]
        public void WinXPAfterFightDependingOnEnemyLevel()
        {
            Character salameche = new Character(100, 50, 30, 20, TYPE.FIRE);
            Character bulbizarre = new Character(90, 60, 10, 200, TYPE.GRASS);
            bulbizarre.Level.AddXP(100);
            Fight f = new Fight(salameche, bulbizarre);
            MegaPunch mp = new MegaPunch();
            Punch p = new Punch();
            f.ExecuteTurn(mp,p);
            Assert.That(salameche.Level.CurrentXP, Is.EqualTo(0));
            Assert.That(salameche.Level.LevelAmount, Is.EqualTo(2));
        }
        
        [Test]
        public void CanLevelUpTwiceOneShot()
        {
            Character salameche = new Character(100, 50, 30, 20, TYPE.FIRE);
            salameche.Level.AddXP(320);
            Assert.That(salameche.Level.CurrentXP, Is.EqualTo(20));
            Assert.That(salameche.Level.LevelAmount, Is.EqualTo(3));
        }
        
        [Test]
        public void CanLevelUpTwiceAfterFight()
        {
            Character salameche = new Character(100, 50, 30, 20, TYPE.FIRE);
            Character bulbizarre = new Character(90, 60, 10, 200, TYPE.GRASS);
            bulbizarre.Level.AddXP(3100);
            Fight f = new Fight(salameche, bulbizarre);
            MegaPunch mp = new MegaPunch();
            Punch p = new Punch();
            f.ExecuteTurn(mp,p);
            Assert.That(salameche.Level.CurrentXP, Is.EqualTo(0));
            Assert.That(salameche.Level.LevelAmount, Is.EqualTo(3));
        }
    }
}
