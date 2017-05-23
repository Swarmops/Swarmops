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

Stable releases are built every six months, at the end of every calendar half-year. Sprints are built every two weeks. Internal builds are built all the time and can be tested at http://dev.swarmops.com/ which doesn't require a login.

This is the plan, at least. "Stable" is a somewhat wide definition at the moment. Rather, Swarmops has a few functions to go to enter Open Beta stage.


Countdown to Open Beta
----------------------

The features missing for the Open Beta, with some sort of feature-completeness, are these:

[ ] enter ledger transactions manually
[ ] send invoices (and receive payment in bitcoin)
[ ] delegate budgets

There will also be many other small improvements added along with these features, for no better reason than their absence being pain points.


Installation
------------

Minimum requirements are Debian Stretch or Ubuntu Xenial (due to systemd requirements; upstart is deprecated). As Debian Stretch hasn't been released at time of writing, Ubuntu Xenial is the only platform Swarmops is currently built for.

If you're daring enough to install a pilot of Swarmops, you're most welcome to do so! Run these commands _as root_ - first, fetch the signing key for the repository:

> `wget -qO- http://packages.swarmops.com/swarmops-packages.gpg.key | apt-key add -`

Then, add the Swarmops repository to your list of software sources, where [your_distribution] below is xenial (Ubuntu) or stretch (Debian):
> `echo deb http://packages.swarmops.com/ [your_distribution] contrib > /etc/apt/sources.list.d/swarmops.list`

Then, run this to install the Swarmops frontend:
> `apt-get update; apt-get install swarmops-frontend`

If you installed onto a clean server, Swarmops will offer to configure Apache to use Swarmops as the default site. If you decline this offer, you can still enable the site by an `a2ensite swarmops` as a suggested configuration is provided. If you prefer to configure this entirely manually, install a new Virtual Host in Apache, a Mono host, pointing at /usr/share/swarmops/frontend as its directory. We're using /usr/bin/mod-mono-server4 as our server. Note the 4 at the end; many configurators are old and will set a 2 there. See /etc/apache2/sites-available/swarmops.conf for a template file.

Navigate to the new site and continue installation from the running site. To complete the install, you will also need to install a backend process, which can (but shouldn't) run on the same machine:

> `apt-get install swarmops-backend`

The packages named as listed above (swarmops-frontend) are the sprint packages, released every two weeks. If you prefer, you can opt for the development builds (swarmops-frontend-internal) or the stable six-month releases (swarmops-frontend-stable) instead. The development builds aren't really recommended unless you're actively contributing to development and want to see new changes running on the development sandbox.

Contributing
------------

No permission necessary, really. Just check in code. The backend is ASP.Net/C# and the frontend (where most of the development happens) is Javascript and jQuery. But if you want to see what's being worked on, feel free to get an account at http://scrum.pirateacademy.eu and join the Swarmops project, and grab tasks from the master list.

Let's take that again, because it's important: **about 90% of development happens in JavaScript and jQuery**, so don't shy away because it looks like a C# backend.

There's also a Facebook group named [Swarmops Developers](https://www.facebook.com/groups/swarmops.developers/) which you may want to join. Yes, Facebook is evil, so give me a better alternative. Until there is one, that's where discussions happen.

License
-------

No, there isn't a "license". This code is completely in the public domain, with the exception of external libraries used. Those are marked as such. In jurisdictions where public domain doesn't exist as a legal concept, the code is under the CC-0 (Creative Commons Zero) license.

That also means that any code _you_ commit to Swarmops, whether by checking in code to this repository or by doing so to forks and then pushing code back here, is irrevocably committed to the public domain.
