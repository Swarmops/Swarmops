Swarmops
========

**Swarmops is a necessary tool to enable any bitcoin-native or decentralized gamechanger.**

Swarmops is an admin system for swarm-type organizations. It's a bureaucracy system for people who thoroughly dislike bureaucracy, so it removes all visibility of it and focuses on the ops aspects. It manages decentralized authority, volunteers, members, activists, budgets, mass communications, expenses, payroll, invoices, and complete financials/bookkeeping.

The goals of _Swarmops_ are three:

* Become the #1 software for organizing swarm activism to effectively change policy,
* Become the #1 software to manage native-bitcoin startups' cashflow and accounting,
* Become the #1 software to run civil liberties resistances in repressive regimes.

Goal #1, _organizing swarm activism:_ the Swedish Pirate Party used a predecessor to Swarmops in its effort to put two representatives in the European Parliament, and could literally not have succeeded without the ability to decentralize authority that Swarmops provided, pushing all the crucial decision-making out to the edges of the organization where the most information was available. It's an administration tool for people who hate paperwork, so it's built to optimize the time available to activism.

Goal #2, _the primary back-end software for bitcoin-native unbanked startups:_ Swarmops does bookkeeping and accounting on fully automatic. Today, there are no services or packages for bitcoin-native and unbanked organizations – for startups which are unbanked by choice. Swarmops seeks to fill that role and provide automatic accounting and cashflow management for such organizations, maintaining hot and cold wallets along with automated invoice and payroll processing. (Imagine invoices being paid on fully automatic, and not needing a €100,000 software package and a fifteen-page bank contract to do so.) There is a huge void to fill here, and Swarmops fills this role in addition to all other back-end management.

Goal #3, _functional software to assist civil liberties resistances in repressive regimes:_ With the swarm functions and the bitcoin-native cash flow in place, a “hidden branch” of organizations can be enabled, where nobody knows the identities of other people in the organization's “hidden branch” swarm except those closest to that individual, but where everybody is still working toward a common goal. Recruiting would take place face-to-face using mobile phones and BitID, and code names would be used for all other purposes. In this way, Swarmops enables large-scale change while able to protect the individuals involved in making that change come about. Lack of information even at the central level provides deniability against rubberhose attacks.

Release schedule
----------------

Stable releases are built every six months, at the end of every calendar half-year. Sprints are built every month. Internal builds are built all the time and can be tested at http://sandbox.swarmops.com/ which doesn't require a login.

This is the plan, at least. "Stable" is a somewhat wide definition at the moment. Rather, Swarmops has a few functions to go to enter Open Beta stage.


Installation
------------

Minimum requirements are Debian Stretch or Ubuntu Xenial (due to systemd requirements; upstart is deprecated). As Debian Stretch hasn't been released at time of writing, Ubuntu Xenial is the only platform Swarmops is currently built for.

If you're daring enough to install a pilot of Swarmops, you're most welcome to do so! Run these commands _as root_ - first, fetch the signing key for the repository:

> `wget -qO- http://packages.swarmops.com/swarmops-packages.gpg.key | apt-key add -`

Then, add the Swarmops repository to your list of software sources, where [your_distribution] below is xenial (Ubuntu) or stretch (Debian):
> `echo deb http://packages.swarmops.com/ [your_distribution] contrib > /etc/apt/sources.list.d/swarmops.list`

Then, run this to install the Swarmops frontend:
> `apt update; apt install swarmops-frontend`

If you installed onto a clean server, Swarmops will offer to configure Apache to use Swarmops as the default site. If you decline this offer, you can still enable the site by an `a2ensite swarmops` as a suggested configuration is provided. If you prefer to configure this entirely manually, install a new Virtual Host in Apache, a Mono host, pointing at /usr/share/swarmops/frontend as its directory. We're using /usr/bin/mod-mono-server4 as our server. Note the 4 at the end; many configurators are old and will set a 2 there. See /etc/apache2/sites-available/swarmops.conf for a template file.

Navigate to the new site and continue installation from the running site. To complete the install, you will also need to install a backend process, which can (but shouldn't) run on the same machine; the frontend communicates with the backend through the database and through TCP port 10944:

> `apt install swarmops-backend`

At one point in the installation process, you will be prompted to copy the file `/etc/swarmops/database.config` from the server running swarmops-*frontend* to the server running swarmops-*backend*. This allows the backend to connect to the database as configured by the installation process. Once you do this, the installation process will detect the running backend and the installation will continue.

The packages named as listed above (swarmops-frontend) are the sprint packages, released on the 5th of every month. If you prefer, you can opt for the development builds (swarmops-frontend-internal) or the stable six-month releases (swarmops-frontend-stable) instead. The development builds aren't really recommended unless you're actively contributing to development and want to see new changes running on the development sandbox.

If you're running into trouble, or are just curious, see the "detailed install instructions" last in this document.


Contributing
------------

No permission necessary, really. Just check in code. The backend is ASP.Net/C# and the frontend (where most of the development happens) is Javascript and jQuery. There's not really a master list beyond this one at present with tasks; getting one to work in GitHub (or GitLab) would be practical. A number of approaches have been tried, none of which have worked out in practice.

Let's take that again, because it's important: **about 90% of development happens in JavaScript and jQuery**, so don't shy away because it looks like a C# backend.

There's also a Facebook group named [Swarmops Developers](https://www.facebook.com/groups/swarmops.developers/) which you may want to join. Yes, Facebook is evil, so give me a better alternative. Until there is one, that's where discussions happen. There's also a little-used [Slack](https://swarmops.slack.com).


License
-------

No, there isn't a "license". This code is completely in the public domain, with the exception of external libraries used. Those are marked as such. In jurisdictions where public domain doesn't exist as a legal concept, the code is under the CC-0 (Creative Commons Zero) license.

That also means that any code _you_ commit to Swarmops, whether by checking in code to this repository or by doing so to forks and then pushing code back here, is irrevocably committed to the public domain.


Beta-2 features progress
------------------------

Beta-2 will be released on December 5, with string freeze on December 2. Its focus is to retool for Bitcoin Cash ~~and/or the Bitcoin 2x fork. This goal is fluid and may change as the strength of the respective forks become clearer.~~

- [x] Database update for Bitcoin Cash balances for addresses
- [ ] Update NBitcoin to handle dual-mode
- [ ] Rewrite the Bitcoin Hotwallet page to display Bitcoin Cash balances (part 1, without conversion to fiat)
- [ ] Rewrite the Bitcoin Echo page to use Bitcoin Cash
- [ ] Rewrite the Bitcoin Donation page; make more resilient to socket failures
- [ ] Write a Pay Invoice page for Bitcoin Cash
- [ ] String Freeze
- [ ] Release


Beta-3 features progress
------------------------

Beta-3 will be released on January 5, 2018, with string freeze on January 2. Its focus will be on Shapeshift integration and ability to receive and send payments in all different cryptocurrencies.

- [x] Import all crypto pairs from Shapeshift and track exchange rates
- [ ] Rewrite the Bitcoin Hotwallet page to display Bitcoin Cash balances (part 2, including conversion to fiat)
- [ ] Enable cryptocurrency as any other currency on entry
- [ ] Enable payment identifiers, with currency
- [ ] Tie payment identifiers to people and suppliers
- [ ] String Freeze


Beta-4 features progress
------------------------

Beta-4 will be released on February 5, 2018, with string freeze on February 2. Its tentative focus will be Bitwala integration and possibly an open API exposure.



Overall Beta features checklist
-----------------------

Here are the features still required to exit beta and declare release:

- [x] enter ledger transactions manually
- [ ] send invoices (and receive payment in bitcoin)
- [ ] delegate budgets
- [ ] self-signup mails
- [ ] ledger-close screen

There will also be many other small improvements added along with these features, for no better reason than their absence being pain points, since the last alpha:

- [x] Proper org settings 
- [ ] HTML/Markdown mail
- [x] Mail Resolver
- [ ] Expensify integration?
- [X] PDF asynchronous interpreter (websocket?)
- [ ] Recurring expenses
- [x] Char encode HTML doc
- [ ] Org descriptions (long, short) on self-signup page
- [ ] Hotwallet payments
- [x] Tech problem box
- [x] Donate sockify
- [x] Live financial numbers
- [ ] Todo box to JSON
- [x] Proper menu highlight
- [X] Basic search
- [x] Account edit spacing
- [ ] Alert to load hotwallet from cold
- [ ] Bitcoin Echo test page (probably needs to be Bitcoin Cash b/c fees)
- [ ] Icons for Validation page
- [x] Favicon New
- [x] Fix Inspect Ledger header (looks bad)
- [x] Clean Login page
- [x] OOBE: Wait for daemons to start
- [x] Impersonation mode for testing
- [x] Expense access
- [ ] Assign role geolock
- [x] Advance line spacing
- [x] Upload Org 16x9 logo
- [ ] Submit invoice anon interface
- [ ] Control messages for invoice progress
- [ ] Pay invoice
- [ ] Controlify financialtransaction to show on balancetx page
- [ ] Close ledger year
- [x] Internal TX account type
- [x] Create TX
- [ ] Download main Ledger
- [X] Fix password reset after refactor


Detailed install instructions
-----------------------------

This is the exact install procedure for a two-server setup -- you could also install on one and the same server:

1. Create two clean Ubuntu Xenial VMs. Call them backend and frontend. They can be in different firewall zones. Install mysql-server on the backend (or on a third server).

2. Install the repository key as described above and run `apt update`.

3. Install the packages "swarmops-frontend" and "swarmops-backend", respectively, on the frontend and backend machine. Make use of as much automation on installing swarmops-frontend as you like, up to and including the autoconfiguration of Apache.

4. Open a browser and navigate to the swarmops-frontend machine, pass the first no-bot check in the install wizards, and enter database server root credentials. This will create a database and provision it with initial data, which takes a couple of minutes. (You can create all of this manually if you're not comfortable with entering root credentials.)

5. The installation will pause here and wait for the backend to come online. This is done by manually copying the now-created `/etc/swarmops/database.config` file from the frontend to the backend machine.

6. As the backend daemon detects the presence of a valid `/etc/swarmops/database.config` file, it will open the database (using the credentials of the file - make sure they're also valid for the backend machine, if you're using machine-level permissions!) and write its presence into the database.

7. The installing frontend process will detect this new entry in the database and proceed to a prompt to create the first user. On entering the credentials, it will login as that user into the Sandbox organization, and the Swarmops instance is operational.

8. From the Dashboard, the Apache will open a websocket to a daemon running on its own machine on port 12172, and that daemon will in turn open a websocket to the backend daemon on port 10944 (configurable in Admin / System-Wide Settings).

9. From there, one can play around with the Sandbox organization, or create one or more live organizations.


If one of these steps fails, in particular if the frontend doesn't get to provision the entire database and bring the first user logged in to the Swarmops Desktop, it is currently recommended to restart from two blank servers. Future code will handle more failure scenarios.

