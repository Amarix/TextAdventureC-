﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Engine;

namespace TextAdventure_AllieBeckman
{
    
    // main game adventure class
    public partial class Adventure : Form
    {
        // an array of player options
        private Player[] players = new Player[3];
        // initalize new player
        private Player _player;
        // initalize new location
        private Location currentLocation;
        // array of game locations
        private Location[,] gameMap = new Location[5, 3];
        // ints to navigate through 2D array
        private int x;
        private int y;
        // array of monsters
        private Monster[] monsters = new Monster[3];
        // monster refrence after random monster is chosen from the array
        private Monster monsterOne;
        // a boolean to tell if a monster is present
        bool isMonster;
        // an array of monster images to display when a monster is present
        Image[] monsterImage = new Image[3];
        // magic object
        private Magic magic;
        // an int to determine that monsters in room have been killed and
        // player can move forward
        int monstKilled;
        // directions form
        Directions directions = new Directions();
        // set temp char choice and an index to count through if
        // user presses next
        int selectionIndex;

        public Adventure()
        {
            InitializeComponent();
            // array of game locations
            gameMap[0, 0] = new Location(2, "Bed", "The bed where you sleep.");
            gameMap[0, 1] = new Location(1, "Home", "This is your home.");
            gameMap[0, 2] = new Location(3, "Window", "You see your field outside.");
            gameMap[1, 0] = new Location(6, "Well", "The well water is refreshing.");
            gameMap[1, 1] = new Location(4, "Field", "A field in front of your house.");
            gameMap[1, 2] = new Location(5, "Shed", "A place to keep your weapons.");
            gameMap[2, 0] = new Location(13, "Field", "Nothing this way.");
            gameMap[2, 1] = new Location(7, "Dungeon Entrance", "You can hear something coming from the cave.");
            gameMap[2, 2] = new Location(14, "Field", "Nothing this way.");
            gameMap[3, 0] = new Location(9, "Blocked Path", "There has been a cave in here.");
            gameMap[3, 1] = new Location(8, "Dungeon Room", "You can see the exit from here.");
            gameMap[3, 2] = new Location(10, "Locked Door", "A locked door");
            gameMap[4, 0] = new Location(12, "Wall", "Nothing this way.");
            gameMap[4, 1] = new Location(11, "Treasure Room", "There is a treasure box here.");
            gameMap[4, 2] = new Location(12, "Wall", "Nothing this way.");

            // set current location for game start
            currentLocation = gameMap[0, 1];

            // an array of monster images to display when monsters appear
            monsterImage[0] = TextAdventure_AllieBeckman.Properties.Resources.imp;
            monsterImage[1] = TextAdventure_AllieBeckman.Properties.Resources.goblin;
            monsterImage[2] = TextAdventure_AllieBeckman.Properties.Resources.troll;

            // set current array index ints
            x = 0;
            y = 1;
            selectionIndex = 0;

            // display current location to screen
            gameDisplay(currentLocation);

            // monster is not present right away
            isMonster = false;
            // monster isn't killed yet
            monstKilled = 0;

            // set players auto magic and weapon choice
            magic = new Magic("fire");

            // show directions
            directions.Show();

            // make the enter button on the keyboard click the enter button on my screen
            this.AcceptButton = enterButton;

            // first choice of char
            characterChoice();
        }


        private void enterButton_Click(object sender, EventArgs e)
        {
            // get current command to lower case
            string cmd = commandTextBox.Text.ToLower();

            // if player tries to use sleep command use sleep method to
            // make sure player is in bedroom
            if (cmd == "sleep")
            {
                sleep();
            }

            if (cmd == "north"|| cmd == "south"|| cmd == "east"|| cmd == "west")
            {
                // call the navigate method
                navigate();
            }

            commandTextBox.SelectAll();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            // close form
            this.Close();
        }

        private void backgroundPicture_Click(object sender, EventArgs e)
        {
            // not used
        }

        private void magicButton_Click(object sender, EventArgs e)
        {
            playerMagicAttack();
        }



// if any of the spell buttons are selected in the shed ---- >
        private void fireSpellBtn_Click(object sender, EventArgs e)
        {
            // set magic stats for fire spell
            magic = new Magic("fire");
            // set spell icon to fire icon
            spellIcon.BackgroundImage = TextAdventure_AllieBeckman.Properties.Resources.fire;
        }

        private void iceSpellBtn_Click(object sender, EventArgs e)
        {
            // set magic stats for ice spell
            magic = new Magic("ice");
            // set spell icon to ice icon
            spellIcon.BackgroundImage = TextAdventure_AllieBeckman.Properties.Resources.ice;
        }

        private void lightningSpellBtn_Click(object sender, EventArgs e)
        {
            // set magic stats for lightning spell
            magic = new Magic("lightning");
            // set spell icon to lightning icon
            spellIcon.BackgroundImage = TextAdventure_AllieBeckman.Properties.Resources.lightning;
        }

        private void rockSpellBtn_Click(object sender, EventArgs e)
        {
            // set magic stats for rock spell
            magic = new Magic("rock");
            // set spell icon to rock icon
            spellIcon.BackgroundImage = TextAdventure_AllieBeckman.Properties.Resources.rock;
        }
// end of spell options/ buttons <-----------



// use potions buttons ---->
        private void healthPotBtn_Click(object sender, EventArgs e)
        {
            useHealthPot();
        }

        private void manaPotBtn_Click(object sender, EventArgs e)
        {
            useManaPot();
        }
// end of use potions buttons <-----



// start of buy potions buttons --->
        private void buyManaBtn_Click(object sender, EventArgs e)
        {
            // first find out if you can afford it with exp
            int exp = _player.ExperiencePts;

            // find out how many pts you already have
            int currentPots = int.Parse(manaPotCnt.Text.ToString());

            // if you can afford it buy one.
            if(exp >= 2)
            {
                // add a new potion to your potions
                manaPotCnt.Text = (currentPots + 1).ToString();
                // subtract the exp cost from players exp
                _player.ExperiencePts = _player.ExperiencePts - 2;

                // run the level system to re determine level
                levelSystem();
            }
        }

        private void buyHealthBtn_Click(object sender, EventArgs e)
        {
            // first find out if you can afford it with exp
            int exp = _player.ExperiencePts;

            // find out how many pts you already have
            int currentPots = int.Parse(healthPotCnt.Text.ToString());

            // if you can afford it buy one.
            if (exp >= 2)
            {
                // add a new potion to your potions
                healthPotCnt.Text = (currentPots + 1).ToString();
                // subtract the exp cost from players exp
                _player.ExperiencePts = _player.ExperiencePts - 2;

                // run the level system to re determine level
                levelSystem();
            }
        }
//End of potion purchases ---------->>>>>>>



// to re-Open directions
        private void directionsBtn_Click(object sender, EventArgs e)
        {
            Directions directions = new Directions();
            directions.Show();
            directions.BringToFront();
        }

        private void attackButton_Click(object sender, EventArgs e)
        {
            // call att method
            playerAttack();
        }

        private void swordBtn_Click(object sender, EventArgs e)
        {
            if (lblWeapon.Text != "Wood Sword")
            {
                // add sword damage to player damage
                _player.Damage = 2;

                // update weapon label
                lblWeapon.Text = "Wood Sword";
            }
            else
            {
                _player.Damage = _player.BaseDamage;
                // update weapon label
                lblWeapon.Text = "None";
            }
        }

        private void axeBtn_Click(object sender, EventArgs e)
        {
            if (lblWeapon.Text != "Axe")
            {
                // add sword damage to player damage
                _player.Damage = 3;

                // update weapon label
                lblWeapon.Text = "Axe";
            }
            else
            {
                _player.Damage = _player.BaseDamage;
                // update weapon label
                lblWeapon.Text = "None";
            }
        }

        private void steelSwordBtn_Click(object sender, EventArgs e)
        {
            if (lblWeapon.Text != "Steel Sword")
            {
                // add sword damage to player damage
                _player.Damage = 5;

                // update weapon label
                lblWeapon.Text = "Steel Sword";
            }else
            {
                _player.Damage = _player.BaseDamage;
                // update weapon label
                lblWeapon.Text = "None";
            }
            
        }


        //PLAYER STATS DISPLAY
        // a method to display the players stats
        public void playerStats(Player player)
        {
            // display stats on window
            lblHealth.Text = _player.CurrentHealth.ToString();
            lblMana.Text = _player.Mana.ToString();
            lblExp.Text = _player.ExperiencePts.ToString();
            lblLevel.Text = _player.Level.ToString();
            lblCrntExp.Text = _player.ExperiencePts.ToString();
        }

        //ATTACK METHODS ---->>>>
        // magic attack
        public void playerMagicAttack()
        {
            // check for mana to use spell
            int currentMana = _player.getMana();

            // get type of spell being used
            string spellType = magic.getType();

            // get mana cost of type
            int spellCost = magic.getMana();

            // get spell types damage
            int spellDamage = magic.getDamage();

            // get current monsters health
            int monstHealth = monsterOne.getHealth();

            // if player can afford the spell
            if (currentMana > spellCost)
            {
                // spell does damange
                monsterOne.CurrentHealth = (monsterOne.CurrentHealth) - spellDamage;

                // player loses mana
                _player.Mana = (_player.Mana) - spellCost;
                // display new ammount of mana
                lblMana.Text = _player.Mana.ToString();

                // display the damage done on the monster
                warningLabel.Text = "A monster appears!"
                    + "\n Name: " + monsterOne.getName()
                    + "\n Health: " + monsterOne.CurrentHealth;

                // if monster dies
                if (monsterOne.CurrentHealth <= 0)
                {
                    // make monster image dissapear
                    monsterPictureBox.Visible = false;
                    // get exp points for monster
                    _player.ExperiencePts = _player.ExperiencePts + monsterOne.getReward();
                    // attack buttons disable
                    battlePanel.Enabled = false;
                    // monster is no longer present
                    isMonster = false;
                    // make monster stats dissappear
                    warningLabel.Text = "Congrats the monster is dead." +
                        "\n You recieve " + monsterOne.getReward() + " exp points.";
                    monstKilled = 1;

                    // can move monster is dead
                    enterButton.Enabled = true;

                    // add the exp and lvl up if possible using this method
                    levelSystem();
                }
                else
                {
                    // if monster does not die it attacks using attack method
                    monsterAttack();
                }
            }
        }

        // melee attack
        public void playerAttack()
        {
            // clear the monsters previous attack
            monstAttlbl.Text = "";

            // get monster health
            int monstHealth = monsterOne.getHealth();

            // get player att damage
            int plrAtt = _player.getDamage();

            // deal damage and calculate monsters new health
            monstHealth = monstHealth - plrAtt;
            monsterOne.CurrentHealth = monstHealth;

            // check if monster died
            if (monstHealth <= 0)
            {
                // make monster image dissapear
                monsterPictureBox.Visible = false;
                // get exp points for monster
                _player.ExperiencePts = _player.ExperiencePts + monsterOne.getReward();
                // attack buttons disable
                battlePanel.Enabled = false;
                // monster is no longer present
                isMonster = false;
                // make monster stats dissappear
                warningLabel.Text = "Congrats the monster is dead." +
                    "\n You recieve " + monsterOne.getReward() + " exp points.";
                monstKilled = 1;

                // can move monster is dead
                enterButton.Enabled = true;

                // add the exp and lvl up if possible using this method
                levelSystem();
            }
            else
            {
                // show monsters new health
                warningLabel.Text = "A monster appears!"
                        + "\n Name: " + monsterOne.getName()
                        + "\n Health: " + monsterOne.CurrentHealth;

                // if monster does not die it attacks
                monsterAttack();
            }
        }

        // monster attack method this can only be called after a monster
        // has been assigned to monsterOne
        public void monsterAttack()
        {
            // get the monsters attack
            int attack = monsterOne.getAttack();

            if (attack <= 0)
            {
                // the monster did no damage so it missed
                string missed = monsterOne.missed();
                monstAttlbl.Text = missed;
            }
            else
            {
                // get players health
                int plrHealth = _player.CurrentHealth;

                // take damage
                plrHealth = plrHealth - attack;

                // display amount of damage
                monstAttlbl.Text = monsterOne.getName() + " attacked and did " + attack + " damage.";

                // make sure player isn't out of health
                if (plrHealth <= 0)
                {
                    // call game over
                    gameOver();
                }
                else
                {
                    // if player has remaining health return new current health
                    _player.CurrentHealth = plrHealth;
                    // display new health to stat bar
                    lblHealth.Text = _player.CurrentHealth.ToString();
                }
            }
        }
        //ATTACK METHODS END ------>>>>>>>


        //EQUIPMENT / SHOP ITEM SELECTION METHOD
        public void equipmentShed()
        {
            // check if you're in the right location
            if (x == 1 && y == 2)
            {
                equipsPanel.Visible = true;
                equipsPanel.Enabled = true;

                // check char level for equip options
                if (_player.Level < 2)
                {
                    // only some equips available base options
                    // are pre set in design

                }
                else if (_player.Level < 5)
                {
                    // open next available weapons and spells
                    lightningSpellBtn.Enabled = true;
                    axeBtn.Enabled = true;
                }
                else
                {
                    // open next available weapons and spells
                    lightningSpellBtn.Enabled = true;
                    rockSpellBtn.Enabled = true;
                    axeBtn.Enabled = true;
                    steelSwordBtn.Enabled = true;

                }
            }
            else
            {
                equipsPanel.Visible = false;
                equipsPanel.Enabled = false;
            }
        }

        //LEVEL AND EXP SYSTEM METHOD
        // a method to calculate level and exp when exp is earned
        public void levelSystem()
        {
            //calculate how much exp it will take to level up
            int expForNxtLvl = _player.Level * 10;

            // how much exp do you need to reach the next level
            int expToNxtLvl = expForNxtLvl - _player.ExperiencePts;
            lblExp.Text = expToNxtLvl.ToString();
            lblCrntExp.Text = _player.ExperiencePts.ToString();

            if (expForNxtLvl <= _player.ExperiencePts)
            {
                // take exp to lvl up and add a lvl
                _player.ExperiencePts = _player.ExperiencePts - expForNxtLvl;
                _player.Level = _player.Level + 1;

                //re-calculate how much exp it will take to level up
                expForNxtLvl = _player.Level * 10;

                // re-calculate how much exp do you need to reach the next level
                expToNxtLvl = expForNxtLvl - _player.ExperiencePts;

                // display new exp current/expToNxtLvl/ and new lvl
                lblExp.Text = expToNxtLvl.ToString();
                lblLevel.Text = _player.Level.ToString();
                lblCrntExp.Text = _player.ExperiencePts.ToString();

                // refill players health and mana and add 5 health and 5 mana to the total
                _player.TotalHealth = _player.TotalHealth + 5;
                _player.TotalMana = _player.TotalMana + 5;

                _player.Mana = _player.TotalMana;
                _player.CurrentHealth = _player.TotalHealth;

                // display refreshed health and mana stats
                lblHealth.Text = _player.CurrentHealth.ToString();
                lblMana.Text = _player.Mana.ToString();
            }

        }

        //ROOMS WHERE MONSTER APPEARS METHOD
        // if monster appears
        public void monsterRoom()
        {
            // get room ID to decide if room is a room with
            // a monster in it
            int i = currentLocation.getID();

            // current rooms where random monsters spawn are first room
            // room with cave in
            // and treasure room.
            if (i == 8 && monstKilled == 0 ||
                i == 9 && monstKilled == 0||
                i == 11 && monstKilled == 0)
            {
                // array of monsters
                monsters[0] = new Monster(5, 5, 2, "Imp", 2, 2);
                monsters[1] = new Monster(10, 10, 1, "Goblin", 3, 3);
                monsters[2] = new Monster(15, 15, 3, "Troll", 5, 5);

                // use a random int to choos a random monster
                // using the random int as the index in the monster array.
                Random r = new Random();
                int monsterIndex = r.Next(0, 3);

                // define new monster
                monsterOne = monsters[monsterIndex];

                // monster is present
                isMonster = true;

                // cant leave until the monster is dead
                enterButton.Enabled = false;

                // display monster stats
                warningLabel.Text = "A monster appears!"
                    + "\n Name: " + monsterOne.getName()
                    + "\n Health: " + monsterOne.CurrentHealth;

                // display monster image
                monsterPictureBox.BackgroundImage = monsterImage[monsterIndex];
                monsterPictureBox.Visible = true;

                // battle panel with attack button etc. active
                battlePanel.Enabled = true;
            }
            else
            {
                warningLabel.Text = "";
            }
        }

        //SCREEN LOCATION DISPLAY METHOD
        // a method to change the location image name and discription
        public void gameDisplay(Location currentLocation)
        {
            LocationDisplay displayCurrent = new LocationDisplay(currentLocation);

            // display location image name and discription
            backgroundPicture.BackgroundImage = displayCurrent.getBackgroundImage();
            discriptionLabel.Text = currentLocation.getDiscription();
            nameLabel.Text = currentLocation.getName();
        }

        //NAVIGATION METHOD (USES OTHER METHODS)
        // a method to hold all directional methods
        public void navigate()
        {
            // take the command string add it to lowercase
            string command = commandTextBox.Text.ToLower();

            // check for monsters
            monsterRoom();

            // can't change rooms if monster is present
            if (isMonster == false)
            {
                monstAttlbl.Text = "";
                // if user types in north
                if (command == "north")
                {
                    // call move north method
                    moveNorth();

                    // display the new location
                    gameDisplay(currentLocation);
                }
                // if user types in south
                else if (command == "south")
                {
                    // call move south method
                    moveSouth();

                    // display the new location
                    gameDisplay(currentLocation);
                }
                // if user types east
                else if (command == "east")
                {
                    // call move east method
                    moveEast();

                    // display new location
                    gameDisplay(currentLocation);
                }
                // if user types west
                else if (command == "west")
                {
                    // call move west method
                    moveWest();

                    // display new location
                    gameDisplay(currentLocation);
                }
            }

            // is this the shed room?
            equipmentShed();

            // once you leave room with monsters if you come back to this room
            // monsters will respawn
            monstKilled = 0;

            // if player is in the bedroom
            int roomID = currentLocation.getID();
            if(roomID == 2)
            {
                warningLabel.Text = "Maybe you should rest for a while.";
            }
        }

        //NORTH METHOD
        public Location moveNorth()
        {
            // if statement to make sure you don't go further than
            // the array limits
            if (x < 4 && y == 1)
            {
                // x was set to the first original location
                // add to it
                x++;
                // make the current location the new location
                // with the new x value as the first index
                Location newLocation = gameMap[x, y];
                currentLocation = newLocation;

                // take away the warning if it's there
                warningLabel.Text = "";
            }
            else
            {
                // cannot go further/ map ends
                warningLabel.Text = "cannot move any further";
            }

            // return new location
            return currentLocation;
        }

        //SOUTH METHOD
        public Location moveSouth()
        {
            // if statement to make sure you don't go further than
            // the array limits
            if (x > 0 && y == 1)
            {
                // x was set to the first original location
                // add to it
                x--;
                // make the current location the new location
                // with the new x value as the first index
                Location newLocation = gameMap[x, y];
                currentLocation = newLocation;
                // take away the warning if it's there
                warningLabel.Text = "";
            }
            else
            {
                // cannot go further/ map ends
                warningLabel.Text = "cannot move any further";
            }

            // return new location
            return currentLocation;
        }

        //EAST METHOD
        public Location moveEast()
        {
            // if statement to make sure you don't go further than
            // the array limits
            if (y > 0)
            {
                // y was set to the first original location
                // add to it
                y--;
                // make the current location the new location
                // with the new y value as the first index
                Location newLocation = gameMap[x, y];
                currentLocation = newLocation;
                // take away the warning if it's there
                warningLabel.Text = "";
            }
            else
            {
                // cannot go further/ map ends
                warningLabel.Text = "cannot move any further";
            }

            // return new location
            return currentLocation;
        }

        //WEST METHOD
        public Location moveWest()
        {
            // if statement to make sure you don't go further than
            // the array limits
            if (y < 2)
            {
                // y was set to the first original location
                // add to it
                y++;
                // make the current location the new location
                // with the new y value as the first index
                Location newLocation = gameMap[x, y];
                currentLocation = newLocation;
                // take away the warning if it's there
                warningLabel.Text = "";
            }
            else
            {
                // cannot go further/ map ends
                warningLabel.Text = "cannot move any further";
            }

            // return new location
            return currentLocation;
        }

        //SLEEP OPTION METHOD
        public void sleep()
        {
            // get current locations ID to find out if it's the bedroom
            int roomID = currentLocation.getID();

            if (roomID == 2)
            {
                // refill players health and mana and display the refills
                _player.CurrentHealth = _player.TotalHealth;
                _player.Mana = _player.TotalMana;

                lblHealth.Text = _player.CurrentHealth.ToString();
                lblMana.Text = _player.Mana.ToString();

                // let player know they slept in warning box
                warningLabel.Text = "You feel well rested.";
            }
        }

        public void useManaPot()
        {
            // get current amount of potions
            int potionAmount;
            try
            {
                potionAmount = int.Parse(manaPotCnt.Text.ToString());
            }
            catch
            {
                potionAmount = 0;
            }

            // if potion available
            if (potionAmount >= 1)
            {
                // add to players mana
                _player.Mana = _player.Mana + 3;
                // remove the potion that was used
                potionAmount = potionAmount - 1;
                manaPotCnt.Text = potionAmount.ToString();

                // display players new mana
                lblMana.Text = _player.Mana.ToString();
            }
        }

        public void useHealthPot()
        {
            // get current amount of potions
            int potionAmount;

            try
            {
                potionAmount = int.Parse(healthPotCnt.Text.ToString());
            }
            catch
            {
                potionAmount = 0;
            }

            // if potion available
            if (potionAmount >= 1)
            {
                // increase players health
                _player.CurrentHealth = _player.CurrentHealth + 3;
                // remove the potion that was used
                potionAmount = potionAmount - 1;
                healthPotCnt.Text = potionAmount.ToString();

                // display players new mana
                lblHealth.Text = _player.CurrentHealth.ToString();
            }
        }

        //GAME OVER METHOD
        public void gameOver()
        {
            // game over message
            warningLabel.Text = "Oh no... "+ "\n You have perished." + "\n Play again?";

            // everything becomes disabled
            battlePanel.Enabled = false;
            enterButton.Enabled = false;
            directionsBtn.Enabled = false;
            exitButton.Enabled = false;
            commandTextBox.Enabled = false;
            statsPanel.Enabled = false;

            // replay or exit button options available
            yesBtn.Visible = true;
            yesBtn.Enabled = true;

            noBtn.Visible = true;
            noBtn.Enabled = true;
        }

        private void yesBtn_Click(object sender, EventArgs e)
        {
            // restart game
            Application.Restart();
        }

        private void noBtn_Click(object sender, EventArgs e)
        {
            // close program
            this.Close();
        }

        private void commandTextBox_TextChanged(object sender, EventArgs e)
        {
            
        }
        // to select all text without having to use mouse
        private void commandTextBox_Enter(object sender, EventArgs e)
        {
            commandTextBox.SelectAll();
        }

        private void commandTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            commandTextBox.SelectAll();
        }


        // char choice before game starts
        public Player temp;
        public Image tempImage;

        private void characterChoice()
        {
            // initalize the array of options
            PlayerOptions newOptions = new PlayerOptions(selectionIndex);

            // set current player choice stats to screen.
            temp = newOptions.getPlayerChoice();
            tempImage = newOptions.getPlayerImage();
            choicePicBox.BackgroundImage = tempImage;
            lblNameChose.Text = temp.getName();
            lblChoseHealth.Text = temp.getTotalHealth().ToString();
            lblChoseMana.Text = temp.getTotalMana().ToString();
            lblChoseStr.Text = temp.getDamage().ToString();

        }

        private void nextBtn_Click(object sender, EventArgs e)
        {
            characterChoice();
            if (selectionIndex < 2)
            {
                selectionIndex++;
            }
            else
            {
                selectionIndex = 0;
            }
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            _player = temp;

            // display stats on window using method
            playerStats(_player);
            playerPictureBox.BackgroundImage = tempImage;

            playerSelectionPanel.Enabled = false;
            playerSelectionPanel.Visible = false;
        }
    }
}
