﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Accounts
{
    public class IPAddressRange
    {

        readonly AddressFamily addressFamily;
        readonly byte[] lowerBytes;
        readonly byte[] upperBytes;

        public IPAddressRange(IPAddress lowerInclusive, IPAddress upperInclusive) {
            
            addressFamily = lowerInclusive.AddressFamily;
            lowerBytes = lowerInclusive.GetAddressBytes();
            upperBytes = upperInclusive.GetAddressBytes();
        }

        public bool IsInRange(IPAddress address) {
            if (address.AddressFamily != addressFamily) {
                return false;
            }

            byte[] addressBytes = address.GetAddressBytes();

            bool lowerBoundary = true, upperBoundary = true;

            for (int i = 0; i < this.lowerBytes.Length &&
            (lowerBoundary || upperBoundary); i++) {
                if ((lowerBoundary && addressBytes[i] < lowerBytes[i]) ||
                    (upperBoundary && addressBytes[i] > upperBytes[i])) {
                    return false;
                }

                lowerBoundary &= (addressBytes[i] == lowerBytes[i]);
                upperBoundary &= (addressBytes[i] == upperBytes[i]);
            }

            return true;
        }

    }
}
